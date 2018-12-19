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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ExaltedCharm.Api.Controllers
{
    [Route("api/exaltedTypes")]
    public class ExaltedTypeController : Controller
    {
        private readonly IRepository _repository;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IUrlHelper _urlHelper;
        private readonly ILogger<ExaltedTypeController> _logger;

        public ExaltedTypeController(IRepository repository, IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService, IUrlHelper urlHelper, ILogger<ExaltedTypeController> logger)
        {
            _repository = repository;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
            _urlHelper = urlHelper;
            _logger = logger;
        }

        [HttpGet(Name = "GetExaltedTypes")]
        public IActionResult GetExaltedTypes(ExaltedTypeResourceParameter exaltedTypeResourceParameter, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<ExaltedType, ExaltedTypeWithoutCastesDto>
           (exaltedTypeResourceParameter.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<CharmTypeDto>(exaltedTypeResourceParameter.Fields))
            {
                return BadRequest();
            }

            var exaltedTypes = _repository
                .GetAllOrderBy<ExaltedType, ExaltedTypeWithoutCastesDto>(exaltedTypeResourceParameter.OrderBy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(exaltedTypeResourceParameter.Name))
            {
                var whereClause = exaltedTypeResourceParameter.Name.Trim().ToLowerInvariant();
                exaltedTypes = exaltedTypes.Where(x => x.Name.ToLowerInvariant() == whereClause);
            }

            var pagedList = PagedList<ExaltedType>.Create(exaltedTypes, exaltedTypeResourceParameter.PageNumber,
                exaltedTypeResourceParameter.PageSize);

            var filters = string.IsNullOrWhiteSpace(exaltedTypeResourceParameter.Name)
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>()
                {
                    {
                        "Name", exaltedTypeResourceParameter.Name
                    }
                };

            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var pageMetaData =
                    exaltedTypeResourceParameter.GeneratePagingMetaData(pagedList, "GetExaltedTypes", _urlHelper);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetaData));

                var links = exaltedTypeResourceParameter.GeneratePagingLinkData("GetExaltedTypes", pagedList.HasNext,
                    pagedList.HasPrevious, _urlHelper, filters);
                var shapedCharmTypes = Mapper.Map<IEnumerable<ExaltedTypeWithoutCastesDto>>(pagedList).ShapeData(exaltedTypeResourceParameter.Fields);

                var shapedCharmTypesWithLinks = shapedCharmTypes.Select(keyword =>
                {
                    var charmTypeAsDictionary = keyword as IDictionary<string, object>;
                    var keywordLink =
                        CreateLinksForExaltedType((int)charmTypeAsDictionary["Id"], exaltedTypeResourceParameter.Fields);
                    charmTypeAsDictionary.Add("links", keywordLink);
                    return charmTypeAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedCharmTypesWithLinks,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var pageMetsaData =
                    exaltedTypeResourceParameter.GeneratePagingMetaData(pagedList, "GetExaltedTypes", _urlHelper, filters,
                        pagedList.HasNext, pagedList.HasPrevious);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));
                return Ok(Mapper.Map<IEnumerable<ExaltedTypeWithoutCastesDto>>(pagedList).ShapeData(exaltedTypeResourceParameter.Fields));
            }
        }

        [HttpGet("{id}", Name = "GetExaltedType")]
        public IActionResult GetExaltedType(int id, string fields, [FromHeader(Name = "Accept")] string mediaType, bool includeCastes = false)
        {

            if (!_typeHelperService.
                TypeHasProperties<KeywordDto>(fields))
            {
                return BadRequest();
            }

            var exaltedType = _repository.GetFirst<ExaltedType>(x => x.Id == id, null, includeCastes ? "Castes" : null);
            if (exaltedType == null)
            {
                return NotFound();
            }

            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var links = CreateLinksForExaltedType(id, fields, includeCastes);
                if (includeCastes)
                {
                    var linkedResourceToReturn = Mapper.Map<ExaltedTypeDto>(exaltedType).ShapeData(fields) as IDictionary<string, object>;

                    linkedResourceToReturn.Add("links", links);

                    return Ok(linkedResourceToReturn);
                }
                else
                {
                    var linkedResourceToReturn = Mapper.Map<ExaltedTypeWithoutCastesDto>(exaltedType).ShapeData(fields) as IDictionary<string, object>;

                    linkedResourceToReturn.Add("links", links);

                    return Ok(linkedResourceToReturn);
                }
            }

            if (includeCastes)
            {
                var exaltedTypeDto = Mapper.Map<ExaltedTypeDto>(exaltedType);
                return Ok(exaltedTypeDto.ShapeData(fields));
            }
            else
            {
                var exaltedTypeDto = Mapper.Map<ExaltedTypeWithoutCastesDto>(exaltedType).ShapeData(fields);
                return Ok(exaltedTypeDto);
            }
        }

        [HttpPost(Name = "CreateExaltedType")]
        public IActionResult CreateCharmType([FromBody] ExaltedTypeForCreationDto exaltedType)
        {
            if (exaltedType == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var exaltedTypeEntity = Mapper.Map<CharmType>(exaltedType);
            _repository.Create(exaltedTypeEntity);

            return !_repository.Save()
                ? StatusCode(500, "A problem happened while handling your request.")
                : CreatedAtRoute("GetExaltedType", new { id = exaltedTypeEntity.Id },
                    Mapper.Map<ExaltedTypeWithoutCastesDto>(exaltedTypeEntity).GenerateLinks(_urlHelper));
        }

        [HttpPost("{id}")]
        public IActionResult BlockExaltedTypeCreation(int id)
        {
            var exaltedType = _repository.GetAll<ExaltedType>().SingleOrDefault(x => x.Id == id);
            return exaltedType == null ? NotFound() : new StatusCodeResult(StatusCodes.Status409Conflict);
        }


        [HttpPut("{id}", Name = "UpdateExaltedType")]
        public IActionResult UpdateExaltedType(int id, [FromBody] ExaltedTypeForUpdateDto exaltedType)
        {
            if (exaltedType == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var exaltedTypeEntity = _repository.GetAll<ExaltedType>().SingleOrDefault(x => x.Id == id);
            if (exaltedTypeEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(exaltedType, exaltedTypeEntity);

            _repository.Update(exaltedTypeEntity);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateExaltedType")]
        public IActionResult PartiallyUpdateExaltedType(int id,
           [FromBody] JsonPatchDocument<ExaltedTypeForUpdateDto> patchDoc,
           [FromHeader(Name = "Accept")] string mediaType)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var exaltedType = _repository.GetAll<ExaltedType>().SingleOrDefault(x => x.Id == id);

            if (exaltedType == null)
            {
                var exaltedTypeDto = new ExaltedTypeForUpdateDto();
                patchDoc.ApplyTo(exaltedTypeDto, ModelState);

                TryValidateModel(exaltedTypeDto);

                if (!ModelState.IsValid)
                {
                    return new UnprocessableEntityObjectResult(ModelState);
                }

                var exaltedTypeToAdd = Mapper.Map<ExaltedType>(exaltedTypeDto);
                exaltedTypeToAdd.Id = id;
                return !_repository.Save()
                    ? StatusCode(500, "A problem happened while handling your request.")
                    : CreatedAtRoute("GetDuration", new { id = exaltedTypeToAdd.Id },
                        mediaType == "application/vnd.exalted.hateoas+json"
                            ? Mapper.Map<ExaltedTypeDto>(exaltedTypeToAdd).GenerateLinks(_urlHelper)
                            : Mapper.Map<ExaltedTypeDto>(exaltedTypeToAdd));
            }

            var exaltedTypeToPatch = Mapper.Map<ExaltedTypeForUpdateDto>(exaltedType);

            patchDoc.ApplyTo(exaltedTypeToPatch, ModelState);

            TryValidateModel(exaltedTypeToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(exaltedTypeToPatch, exaltedType);

            _repository.Update(exaltedType);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();

        }

        [HttpDelete("{id}", Name = "DeleteExaltedType")]
        public IActionResult DeleteExaltedType(int id)
        {
            var exaltedType = _repository.GetAll<ExaltedType>().SingleOrDefault(x => x.Id == id);
            if (exaltedType == null)
            {
                return NotFound();
            }

            _repository.Delete(exaltedType);

            _logger.LogInformation(100, $"Durantion {id} - {exaltedType.Name} deleted");

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForExaltedType(int id, string fields, bool includeCastes = false)
        {
            var links = new List<LinkDto>
            {
                string.IsNullOrWhiteSpace(fields)
                    ? new LinkDto(_urlHelper.Link("GetExaltedType", new {id = id, includeCastes = includeCastes}), "self", "GET")
                    : new LinkDto(_urlHelper.Link("GetExaltedType", new {id = id, includeCastes = includeCastes, fields = fields}), "self", "GET"),
                new LinkDto(_urlHelper.Link("DeleteExaltedType", new {id = id}), "delete_exaltedType",
                    "DELETE"),
                new LinkDto(_urlHelper.Link("UpdateExaltedType", new {id = id}), "update_exaltedtype",
                    "PUT"),
                new LinkDto(_urlHelper.Link("PartiallyUpdateExaltedType", new {id = id}),
                    "partially_update_exaltedtype",
                    "PATCH")
            };

            return links;
        }
    }
}