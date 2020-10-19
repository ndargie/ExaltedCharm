using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Extensions;
using ExaltedCharm.Api.Helpers;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ExaltedCharm.Api.Controllers
{

    [Route("api/WeaponTags")]
    public class WeaponTagController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger<WeaponTagController> _logger;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;

        public WeaponTagController(IRepository repository, ILogger<WeaponTagController> logger, IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService, ITypeHelperService typeHelperService)
        {
            _repository = repository;
            _logger = logger;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetWeaponTags")]
        public IActionResult GetWeaponTags(WeaponTagResourceParameter weaponTagResourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<WeaponTag, WeaponTagDto>
                (weaponTagResourceParameter.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.
                TypeHasProperties<WeaponTagDto>(weaponTagResourceParameter.Fields))
            {
                return BadRequest();
            }

            var weaponTags = _repository.GetAllOrderBy<WeaponTag, WeaponTagDto>(weaponTagResourceParameter.OrderBy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(weaponTagResourceParameter.Name))
            {
                var whereClause = weaponTagResourceParameter.Name.Trim().ToLowerInvariant();
                weaponTags = weaponTags.Where(x => x.Name.ToLowerInvariant() == whereClause);
            }

            var pagedList = PagedList<WeaponTag>.Create(weaponTags, weaponTagResourceParameter.PageNumber,
                weaponTagResourceParameter.PageSize);

            var filters = string.IsNullOrWhiteSpace(weaponTagResourceParameter.Name)
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>()
                {
                    {
                        "Name", weaponTagResourceParameter.Name
                    }
                };

            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var pageMetsaData =
                    weaponTagResourceParameter.GeneratePagingMetaData(pagedList, "GetWeaponTags", _urlHelper);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));
                var links = weaponTagResourceParameter.GeneratePagingLinkData("GetWeaponTags", pagedList.HasNext,
                    pagedList.HasPrevious, _urlHelper, filters);
                var shapedAbilities = Mapper.Map<IEnumerable<WeaponTagDto>>(pagedList).ShapeData(weaponTagResourceParameter.Fields);
                var shapedAbilitiesWithLinks = shapedAbilities.Select(ability =>
                {
                    var abilitiesAsDictionary = ability as IDictionary<string, object>;
                    var abilitylinks =
                        CreateLinksForWeaponTags((int)abilitiesAsDictionary["Id"], weaponTagResourceParameter.Fields);
                    abilitiesAsDictionary.Add("links", abilitylinks);
                    return abilitiesAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedAbilitiesWithLinks,
                    links
                };

                return Ok(linkedCollectionResource);

            }
            else
            {
                var pageMetsaData =
                    weaponTagResourceParameter.GeneratePagingMetaData(pagedList, "GetWeaponTags", _urlHelper, filters,
                        pagedList.HasNext, pagedList.HasPrevious);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));
                return Ok(Mapper.Map<IEnumerable<WeaponTagDto>>(pagedList).ShapeData(weaponTagResourceParameter.Fields));
            }
        }

        [HttpGet("{id}", Name = "GetWeaponTag")]
        public IActionResult GetWeaponTag(int id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.
                TypeHasProperties<WeaponTagDto>(fields))
            {
                return BadRequest();
            }

            var weaponTag = _repository.GetAll<WeaponTag>().SingleOrDefault(x => x.Id == id);

            if (weaponTag == null)
            {
                return NotFound();
            }

            var weaponTagDto = Mapper.Map<WeaponTagDto>(weaponTag);


            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var links = CreateLinksForWeaponTags(id, fields);

                var linkedResourceToReturn = weaponTagDto.ShapeData(fields) as IDictionary<string, object>;

                linkedResourceToReturn.Add("links", links);

                return Ok(linkedResourceToReturn);
            }
            return Ok(weaponTagDto.ShapeData(fields));
        }

        [HttpPost(Name = "CreateWeaponTag")]
        public IActionResult CreateWeaponTag([FromBody] SaveWeaponTagDto weaponTag,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (weaponTag == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var weaponTagEntity = Mapper.Map<WeaponTag>(weaponTag);
            _repository.Create(weaponTagEntity);

            return !_repository.Save() ? StatusCode(500, "A problem happened while handling your request.")
                : CreatedAtRoute("GetWeaponTag", new { id = weaponTagEntity.Id },
                mediaType == "application/vnd.exalted.hateoas+json"
                    ? Mapper.Map<WeaponTagDto>(weaponTagEntity).CreateLinksForWeaponTag(_urlHelper)
                    : Mapper.Map<WeaponTagDto>(weaponTagEntity));
        }

        [HttpPost("{id}")]
        public IActionResult BlockWeaponTagCreation(int id)
        {
            var weaponTag = _repository.GetAll<WeaponTag>().SingleOrDefault(x => x.Id == id);
            return weaponTag == null ? NotFound() : new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        [HttpPut("{id}", Name = "UpdateWeaponTag")]
        public IActionResult UpdateWeaponTag(int id, [FromBody] WeaponTagForUpdate weaponTag)
        {
            if (weaponTag == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var weaponTagEntity = _repository.GetAll<WeaponTag>().SingleOrDefault(x => x.Id == id);
            if (weaponTagEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(weaponTag, weaponTagEntity);

            _repository.Update(weaponTagEntity);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateWeaponTag")]
        public IActionResult PartiallyUpdateWeaponTag(int id,
          [FromBody] JsonPatchDocument<WeaponTagForUpdate> patchDoc,
          [FromHeader(Name = "Accept")] string mediaType)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var weaponTag = _repository.GetAll<WeaponTag>().SingleOrDefault(x => x.Id == id);

            if (weaponTag == null)
            {
                var weaponTagDto = new WeaponTagForUpdate();
                patchDoc.ApplyTo(weaponTagDto, ModelState);

                TryValidateModel(weaponTagDto);

                if (!ModelState.IsValid)
                {
                    return new UnprocessableEntityObjectResult(ModelState);
                }

                var weaponTagToAdd = Mapper.Map<Duration>(weaponTagDto);
                weaponTagToAdd.Id = id;
                return !_repository.Save()
                    ? StatusCode(500, "A problem happened while handling your request.")
                    : CreatedAtRoute("GetDuration", new { id = weaponTagToAdd.Id },
                        mediaType == "application/vnd.exalted.hateoas+json"
                            ? Mapper.Map<WeaponTagDto>(weaponTagToAdd).CreateLinksForWeaponTag(_urlHelper)
                            : Mapper.Map<WeaponTagDto>(weaponTagToAdd));
            }

            var weaponTagToPatch = Mapper.Map<WeaponTagForUpdate>(weaponTag);

            patchDoc.ApplyTo(weaponTagToPatch, ModelState);

            TryValidateModel(weaponTagToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(weaponTagToPatch, weaponTag);

            _repository.Update(weaponTag);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteWeaponTag")]
        public IActionResult DeleteWeaponTag(int id)
        {
            var weaponTag = _repository.GetAll<WeaponTag>().SingleOrDefault(x => x.Id == id);
            if (weaponTag == null)
            {
                return NotFound();
            }

            weaponTag.Weapons.Clear();
            _repository.Update(weaponTag);
            _repository.Delete(weaponTag);

            _logger.LogInformation(100, $"WeaponTag {id} - {weaponTag.Name} deleted");

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForWeaponTags(int id, string fields)
        {
            var links = new List<LinkDto>
            {
                string.IsNullOrWhiteSpace(fields)
                    ? new LinkDto(_urlHelper.Link("GetWeaponTags", new {id}), "self", "GET")
                    : new LinkDto(_urlHelper.Link("GetWeaponTags", new { id, fields}), "self", "GET"),
                new LinkDto(_urlHelper.Link("DeleteWeaponTag", new {id}), "delete_weapontag",
                    "DELETE"),
                new LinkDto(_urlHelper.Link("UpdateWeaponTag", new {id}), "update_weapontag",
                    "PUT"),
                new LinkDto(_urlHelper.Link("PartiallyUpdateWeaponTag", new {id}),
                    "partially_update_weapontag",
                    "PATCH")
            };

            return links;
        }
    }
}