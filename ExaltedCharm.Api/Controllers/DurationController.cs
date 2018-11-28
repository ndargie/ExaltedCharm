using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Helpers;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ExaltedCharm.Api.Controllers
{
    [Route("api/Durations")]
    public class DurationController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger<DurationController> _logger;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;

        public DurationController(IRepository repository, 
            ILogger<DurationController> logger,
            IUrlHelper urlHelper, 
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _repository = repository;
            _logger = logger;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetDurations")]
        public IActionResult GetDurations(DurationResourceParameter durationResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<Duration, DurationDto>
                (durationResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.
                TypeHasProperties<DurationDto>(durationResourceParameters.Fields))
            {
                return BadRequest();
            }
           
            var durations = _repository.GetAllOrderBy<Duration, DurationDto>(durationResourceParameters.OrderBy).AsQueryable();
            if (!string.IsNullOrWhiteSpace(durationResourceParameters.Name))
            {
                var whereClause = durationResourceParameters.Name.Trim().ToLowerInvariant();
                durations = durations.Where(x => x.Name.ToLowerInvariant() == whereClause);
            }
            var pagedList = PagedList<Duration>.Create(durations, durationResourceParameters.PageNumber,
                durationResourceParameters.PageSize);
            var filters = string.IsNullOrWhiteSpace(durationResourceParameters.Name)
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>()
                {
                    {
                        "Name", durationResourceParameters.Name
                    }
                };
            var pageMetsaData =
                durationResourceParameters.GeneratePagingMetaData(pagedList, "GetDurations", _urlHelper, filters);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));

            return Ok( Mapper.Map<IEnumerable<DurationDto>>(pagedList).Select(x =>
            {
                x = x.CreateLinksForDuration(_urlHelper);
                return x;
            }).ShapeData(durationResourceParameters.Fields));
        }

        [HttpGet("{id}", Name = "GetDuration")]
        public IActionResult GetDuration(int id, [FromQuery] string fields)
        {

            if (!_typeHelperService.
                TypeHasProperties<DurationDto>(fields))
            {
                return BadRequest();
            }

            var duration = _repository.GetAll<Duration>().SingleOrDefault(x => x.Id == id);
            if (duration == null)
            {
                return NotFound();
            }

            var durationDto = Mapper.Map<DurationDto>(duration);

            var links = CreateLinksForDuration(id, fields);

            var linkedResourceToReturn = durationDto.ShapeData(fields) as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok();
        }

        [HttpGet("{id}/Charms", Name = "GetCharmsForDuration")]
        public IActionResult GetCharmsForDuration(int id)
        {
            var duration = _repository.GetAll<Duration>()
                .Include(x => x.Charms)
                .SingleOrDefault(x => x.Id == id);
            if (duration == null)
            {
                return NotFound();
            }

            var charms = Mapper.Map<IEnumerable<CharmDto>>(duration.Charms).Select(x =>
            {
                return x = x.GenerateLinks(_urlHelper);
            });
            var wrapper = new LinkedCollectionResourceWrapperDto<CharmDto>(charms);
            return Ok(wrapper.CreateDurationLinksForCharms(_urlHelper));
        }

        [HttpPost]
        public IActionResult CreateDuration([FromBody] SaveDurationDto duration)
        {

            if (duration == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var durationEntity = Mapper.Map<Duration>(duration);
            _repository.Create(durationEntity);
            return !_repository.Save() ?
                StatusCode(500, "A problem happened while handling your request.") :
                CreatedAtRoute("GetDuration", new { id = durationEntity.Id }, 
                    Mapper.Map<DurationDto>(durationEntity).CreateLinksForDuration(_urlHelper));
        }

        [HttpPost("{id}")]
        public IActionResult BlockDurationCreation(int id)
        {
            var duration = _repository.GetAll<Duration>().SingleOrDefault(x => x.Id == id);
            return duration == null ? NotFound() : new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        [HttpPut("{id}", Name = "UpdateDuration")]
        public IActionResult UpdateDuration(int id, [FromBody] DurationForUpdate duration)
        {
            if (duration == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var durationEntity = _repository.GetAll<Duration>().SingleOrDefault(x => x.Id == id);
            if (durationEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(duration, durationEntity);

            _repository.Update(durationEntity);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateDuration")]
        public IActionResult PartiallyUpdateDuration(int id,
            [FromBody] JsonPatchDocument<DurationForUpdate> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var duration = _repository.GetAll<Duration>().SingleOrDefault(x => x.Id == id);

            if (duration == null)
            {
                var durationDto = new DurationForUpdate();
                patchDoc.ApplyTo(durationDto, ModelState);

                TryValidateModel(durationDto);

                if (!ModelState.IsValid)
                {
                    return  new UnprocessableEntityObjectResult(ModelState);
                }

                var durationToAdd = Mapper.Map<Duration>(durationDto);
                durationToAdd.Id = id;
                return !_repository.Save()
                    ? StatusCode(500, "A problem happened while handling your request.")
                    : CreatedAtRoute("GetDuration", new { id = durationToAdd.Id },
                        AutoMapper.Mapper.Map<DurationDto>(durationToAdd));
            }

            var durationToPatch = Mapper.Map<DurationForUpdate>(duration);

            patchDoc.ApplyTo(durationToPatch, ModelState);

            TryValidateModel(durationToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(durationToPatch, duration);

            _repository.Update(duration);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();

        }

        [HttpDelete("{id}", Name = "DeleteDuration")]
        public IActionResult DeleteDuration(int id)
        {
            var duration = _repository.GetAll<Duration>().SingleOrDefault(x => x.Id == id);
            if (duration == null)
            {
                return NotFound();
            }

            _repository.Delete(duration);

            _logger.LogInformation(100, $"Durantion {id} - {duration.Name} deleted");

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForDuration(int id, string fields)
        {
            var links = new List<LinkDto>
            {
                string.IsNullOrWhiteSpace(fields)
                    ? new LinkDto(_urlHelper.Link("GetDuration", new {id = id}), "self", "GET")
                    : new LinkDto(_urlHelper.Link("GetDuration", new {id = id, fields = fields}), "self", "GET"),
                new LinkDto(_urlHelper.Link("GetCharmsForDuration", new {id = id}),
                    "get-charms", "GET"),
                new LinkDto(_urlHelper.Link("DeleteDuration", new {id = id}), "delete_duration",
                    "DELETE"),
                new LinkDto(_urlHelper.Link("UpdateDuration", new {id = id}), "update_duration",
                    "PUT"),
                new LinkDto(_urlHelper.Link("PartiallyUpdateDuration", new {id = id}),
                    "partially_update_duration",
                    "PATCH")
            };

            return links;

        }

       
    }
}