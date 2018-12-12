using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Helpers;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ExaltedCharm.Api.Controllers
{

    [Route("api/charmTypes")]
    public class CharmTypeController : Controller
    {
        private readonly IRepository _charmTypeRepository;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;

        public CharmTypeController(IRepository charmTypeRepository, IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService, ITypeHelperService typeHelperService)
        {
            _charmTypeRepository = charmTypeRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetCharmTypes")]
        public IActionResult GetCharmTypes(CharmTypeResourceParameter charmTypeResourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<CharmType, CharmTypeDto>
                (charmTypeResourceParameter.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<CharmTypeDto>(charmTypeResourceParameter.Fields))
            {
                return BadRequest();
            }

            var charmTypes = _charmTypeRepository
                .GetAllOrderBy<CharmType, CharmTypeDto>(charmTypeResourceParameter.OrderBy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(charmTypeResourceParameter.Name))
            {
                var whereClause = charmTypeResourceParameter.Name.Trim().ToLowerInvariant();
                charmTypes = charmTypes.Where(x => x.Name.ToLowerInvariant() == whereClause);
            }

            var pagedList = PagedList<CharmType>.Create(charmTypes, charmTypeResourceParameter.PageNumber,
                charmTypeResourceParameter.PageSize);

            var filters = string.IsNullOrWhiteSpace(charmTypeResourceParameter.Name)
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>()
                {
                    {
                        "Name", charmTypeResourceParameter.Name
                    }
                };

            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var pageMetsaData =
                    charmTypeResourceParameter.GeneratePagingMetaData(pagedList, "GetCharmTypes", _urlHelper);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));

                var links = charmTypeResourceParameter.GeneratePagingLinkData("GetCharmTypes", pagedList.HasNext,
                    pagedList.HasPrevious, _urlHelper, filters);
                var shapedCharmTypes = Mapper.Map<IEnumerable<CharmTypeDto>>(pagedList).ShapeData(charmTypeResourceParameter.Fields);

                var shapedCharmTypesWithLinks = shapedCharmTypes.Select(keyword =>
                {
                    var charmTypeAsDictionary = keyword as IDictionary<string, object>;
                    var keywordLink =
                        CreateLinksForCharmType((int)charmTypeAsDictionary["Id"], charmTypeResourceParameter.Fields);
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
                    charmTypeResourceParameter.GeneratePagingMetaData(pagedList, "GetCharmTypes", _urlHelper, filters,
                        pagedList.HasNext, pagedList.HasPrevious);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));
                return Ok(Mapper.Map<IEnumerable<CharmTypeDto>>(pagedList).ShapeData(charmTypeResourceParameter.Fields));
            }
        }

        [HttpGet("{id}", Name = "GetCharmType")]
        public IActionResult GetCharmType(int id, string fields, [FromHeader(Name = "Accept")] string mediaType, bool includeCharms = false)
        {

            if (!_typeHelperService.
                TypeHasProperties<CharmTypeDto>(fields))
            {
                return BadRequest();
            }

            var charmType = _charmTypeRepository.GetFirst<CharmType>(x => x.Id == id, null, includeCharms ? "Charms" : null);
            if (charmType == null)
            {
                return NotFound();
            }

            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var links = CreateLinksForCharmType(id, fields, includeCharms);
                if (includeCharms)
                {
                    var linkedResourceToReturn = Mapper.Map<CharmTypeDto>(charmType).ShapeData(fields) as IDictionary<string, object>;

                    linkedResourceToReturn.Add("links", links);

                    return Ok(linkedResourceToReturn);
                }
                else
                {
                    var linkedResourceToReturn = Mapper.Map<CharmTypeWithoutCharmsDto>(charmType).ShapeData(fields) as IDictionary<string, object>;

                    linkedResourceToReturn.Add("links", links);

                    return Ok(linkedResourceToReturn);
                }
            }

            if (includeCharms)
            {
                var charmTypeDto = Mapper.Map<CharmTypeDto>(charmType);
                return Ok(charmTypeDto.ShapeData(fields));
            }
            else
            {
                var charmTypeDto = Mapper.Map<CharmTypeWithoutCharmsDto>(charmType).ShapeData(fields);
                return Ok(charmTypeDto);
            }
        }

        [HttpPost(Name = "CreateCharmType")]
        public IActionResult CreateCharmType([FromBody] CharmTypeCreationDto charmType)
        {
            if (charmType == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var charmTypeEntity = Mapper.Map<CharmType>(charmType);
            _charmTypeRepository.Create(charmTypeEntity);

            return !_charmTypeRepository.Save()
                ? StatusCode(500, "A problem happened while handling your request.")
                : CreatedAtRoute("GetCharmType", new { id = charmTypeEntity.Id },
                    AutoMapper.Mapper.Map<CharmTypeDto>(charmTypeEntity).GenerateLinks(_urlHelper));
        }

        [HttpPut("{id}", Name = "UpdateCharmType")]
        public IActionResult UpdateCharmType(int id, CharmTypeUpdateDto charmType)
        {
            if (charmType == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var charmTypeEntity = _charmTypeRepository.GetAll<CharmType>().SingleOrDefault(x => x.Id == id);
            if (charmTypeEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(charmType, charmTypeEntity);

            _charmTypeRepository.Update(charmTypeEntity);

            if (!_charmTypeRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCharmType")]
        public IActionResult PartiallyUpdateCharmType(int id, [FromBody] JsonPatchDocument<CharmTypeUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var charmType = _charmTypeRepository.GetAll<CharmType>().SingleOrDefault(x => x.Id == id);

            if (charmType == null)
            {
                var charmTypeDto = new CharmTypeUpdateDto();
                patchDoc.ApplyTo(charmTypeDto, ModelState);

                TryValidateModel(charmTypeDto);

                if (!ModelState.IsValid)
                {
                    return new UnprocessableEntityObjectResult(ModelState);
                }

                var charmTypeToAdd = AutoMapper.Mapper.Map<CharmType>(charmTypeDto);
                charmTypeToAdd.Id = id;

                return !_charmTypeRepository.Save()
                    ? StatusCode(500, "A problem happened while handling your request.")
                    : CreatedAtRoute("GetCharmType", new { id = charmTypeToAdd.Id },
                        AutoMapper.Mapper.Map<CharmTypeDto>(charmTypeToAdd).GenerateLinks(_urlHelper));
            }

            var charmTypeToPatch = AutoMapper.Mapper.Map<CharmTypeUpdateDto>(charmType);

            patchDoc.ApplyTo(charmTypeToPatch, ModelState);

            TryValidateModel(charmTypeToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            AutoMapper.Mapper.Map(charmTypeToPatch, charmType);

            _charmTypeRepository.Update(charmType);

            if (!_charmTypeRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCharmType")]
        public IActionResult DeleteCharmType(int id)
        {
            var charmType = _charmTypeRepository.GetAll<CharmType>().Include(x => x.Charms).SingleOrDefault(x => x.Id == id);
            if (charmType == null)
            {
                return NotFound();
            }
            _charmTypeRepository.Delete(charmType);
            if (!_charmTypeRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForCharmType(int id, string fields, bool includeCharms = false)
        {
            var links = new List<LinkDto>
            {
                string.IsNullOrWhiteSpace(fields)
                    ? new LinkDto(_urlHelper.Link("GetCharmType", new {id = id, includeCharms = includeCharms}), "self", "GET")
                    : new LinkDto(_urlHelper.Link("GetCharmType", new {id = id, includeCharms = includeCharms, fields = fields}), "self", "GET"),
                new LinkDto(_urlHelper.Link("DeleteCharmType", new {id = id}), "delete_charmtype",
                    "DELETE"),
                new LinkDto(_urlHelper.Link("GetCharmsForCharmType", new {charmTypeId = id}), "get_charms", "GET"),
                new LinkDto(_urlHelper.Link("UpdateCharmType", new {id = id}), "update_charmtype",
                    "PUT"),
                new LinkDto(_urlHelper.Link("PartiallyUpdateCharmType", new {id = id}),
                    "partially_update_charmtype",
                    "PATCH")
            };

            return links;
        }
    }
}
