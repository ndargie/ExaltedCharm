using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Helpers;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ExaltedCharm.Api.Controllers
{
    [Route("api/charmTypes")]
    public class CharmController : Controller
    {
        private readonly ILogger<CharmController> _logger;
        private readonly IMailService _localMailService;
        private readonly IRepository _repository;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;

        public CharmController(ILogger<CharmController> logger,
            IMailService localMailService,
            IRepository repository, IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _logger = logger;
            _localMailService = localMailService;
            _repository = repository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet("{charmTypeId}/charms", Name = "GetCharmsForCharmType")]
        public IActionResult GetCharms(int charmTypeId, CharmResourceParameter charmResourceParameter, [FromHeader(Name = "Accept")] string mediaType)
        {

            if (!_propertyMappingService.ValidMappingExistsFor<CharmType, CharmTypeDto>
                (charmResourceParameter.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<CharmDto>(charmResourceParameter.Fields))
            {
                return BadRequest();
            }

            try
            {
                if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
                {
                    _logger.LogInformation($"Charmtype with id {charmTypeId} wasn't found when accessing charms");
                    return NotFound();
                }

                var charms = _repository.GetAllOrderBy<Charm, CharmDto>(charmResourceParameter.OrderBy)
                    .Where(x => x.CharmTypeId == charmTypeId).AsQueryable();

                if (!string.IsNullOrWhiteSpace(charmResourceParameter.Name))
                {
                    var whereClause = charmResourceParameter.Name.Trim().ToLowerInvariant();
                    charms = charms.Where(x => x.Name.ToLowerInvariant() == whereClause);
                }


                var pagedList = PagedList<Charm>.Create(charms, charmResourceParameter.PageNumber,
                    charmResourceParameter.PageSize);

                var filters = string.IsNullOrWhiteSpace(charmResourceParameter.Name)
                    ? new Dictionary<string, string>()
                    : new Dictionary<string, string>()
                    {
                        {
                            "Name", charmResourceParameter.Name
                        }
                    };

                if (mediaType == "application/vnd.exalted.hateoas+json")
                {
                    var pageMetsaData =
                        charmResourceParameter.GeneratePagingMetaData(pagedList, "GetCharmsForCharmType", _urlHelper, filters);

                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));

                    var links = charmResourceParameter.GeneratePagingLinkData("GetCharmsForCharmType", pagedList.HasNext,
                        pagedList.HasPrevious, _urlHelper, filters);
                    var shapedCharm = Mapper.Map<IEnumerable<CharmDto>>(pagedList).ShapeData(charmResourceParameter.Fields);

                    var shapedCharmTypesWithLinks = shapedCharm.Select(charm =>
                    {
                        var charmAsDictionary = charm as IDictionary<string, object>;
                        var charmLink =
                            CreateLinksForCharm((int)charmAsDictionary["Id"], charmTypeId, charmResourceParameter.Fields);
                        charmAsDictionary.Add("links", charmLink);
                        return charmAsDictionary;
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
                        charmResourceParameter.GeneratePagingMetaData(pagedList, "GetCharmsForCharmType", _urlHelper, filters,
                            pagedList.HasNext, pagedList.HasPrevious);
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));

                    return Ok(Mapper.Map<IEnumerable<CharmDto>>(pagedList).ShapeData(charmResourceParameter.Fields));
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting charms for charmtype {charmTypeId}", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{charmTypeId}/charms/{id}", Name = "GetCharm")]
        public IActionResult GetCharm(int charmTypeId, int id, string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.
                TypeHasProperties<KeywordDto>(fields))
            {
                return BadRequest();
            }

            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                _logger.LogInformation($"Charmtype with id {charmTypeId} wasn't found when accessing charms");
                return NotFound();
            }

            var charm = _repository.GetAll<Charm>().SingleOrDefault(x => x.CharmTypeId == charmTypeId && x.Id == id);
            if (charm == null)
            {
                return NotFound();
            }

            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var links = CreateLinksForCharm(id, charmTypeId, fields);
                var linkedResourceToReturn = Mapper.Map<CharmDto>(charm).ShapeData(fields) as IDictionary<string, object>;

                linkedResourceToReturn.Add("links", links);

                return Ok(linkedResourceToReturn);
            }

            return Ok(Mapper.Map<CharmDto>(charm).ShapeData(fields));
        }

        [HttpPost("{charmTypeId}/charms")]
        public IActionResult CreateCharm(int charmTypeId,
            [FromBody] CharmCreationDto charm)
        {
            if (charm == null)
            {
                return BadRequest();
            }

            if (charm.Name == charm.Description)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var finalCharm = AutoMapper.Mapper.Map<Charm>(charm);

            var charmType = _repository.GetFirst<CharmType>(x => x.Id == charmTypeId);

            charmType.Charms.Add(finalCharm);
            if (!_repository.Save())
            {
                StatusCode(500, "A problem happened while handling your request.");
            }

            return CreatedAtRoute("GetCharm", new { charmTypeId, id = finalCharm.Id },
                Mapper.Map<CharmDto>(finalCharm).GenerateLinks(_urlHelper));
        }

        [HttpPut("{charmTypeId}/charms/{id}", Name = "UpdateCharm")]
        public IActionResult UpdateCharm(int charmTypeId, int id, [FromBody] CharmUpdateDto charm)
        {
            if (charm == null)
            {
                return BadRequest();
            }

            if (charm.Name == charm.Description)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var charmEntity = _repository.GetAll<Charm>().SingleOrDefault(x => x.CharmTypeId == charmTypeId && x.Id == id);
            if (charmEntity == null)
            {
                var charmToAdd = Mapper.Map<Charm>(charm);
                charmToAdd.Id = id;
                var charmType = _repository.GetAll<CharmType>().Single(x => x.Id == charmTypeId);
                charmType.Charms.Add(charmToAdd);
                return _repository.Save()
                    ? StatusCode(500, "A problem happend while handling your request")
                    : CreatedAtRoute("GetCharm", new { charmTypeId, id = charmToAdd.Id },
                        Mapper.Map<CharmDto>(charmToAdd).GenerateLinks(_urlHelper));
            }

            Mapper.Map(charm, charmEntity);
            _repository.Update(charmEntity);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happend while handling your request");
            }

            return NoContent();
        }

        [HttpPatch("{charmTypeId}/charms/{id}", Name = "PartiallyUpdateCharm")]
        public IActionResult PartiallyUpdateCharm(int charmTypeId, int id, [FromBody] JsonPatchDocument<CharmUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var charmEntity = _repository.GetAll<Charm>().SingleOrDefault(x => x.CharmTypeId == charmTypeId && x.Id == id);
            if (charmEntity == null)
            {
                return NotFound();
            }

            var charmToPatch = Mapper.Map<CharmUpdateDto>(charmEntity);

            patchDocument.ApplyTo(charmToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (charmToPatch.Description == charmToPatch.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name");
            }

            TryValidateModel(charmToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(charmToPatch, charmEntity);
            _repository.Update(charmEntity);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

            return NoContent();
        }

        [HttpDelete("{charmTypeId}/charms/{id}", Name = "DeleteCharm")]
        public IActionResult DeleteCharm(int charmTypeId, int id)
        {
            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var charmEntity = _repository.GetAll<Charm>().SingleOrDefault(x => x.CharmTypeId == charmTypeId && x.Id == id);
            if (charmEntity == null)
            {
                return NotFound();
            }

            _repository.Delete(charmEntity); ;
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

            _localMailService.Send("Charm deleted.",
                $"Charm {charmEntity.Name} with id {charmEntity.Id} was deleted.");

            return NoContent();
        }

        [HttpGet("{charmTypeId}/charms/{id}/keywords", Name = "GetKeywordsForCharm")]
        public IActionResult GetKeywords(int charmTypeId, int id)
        {
            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var charm = _repository.GetFirst<Charm>(x => x.Id == id && x.CharmTypeId == charmTypeId, null, "Keywords.Keyword");
            if (charm == null)
            {
                return NotFound();
            }

            var keywords = Mapper.Map<IEnumerable<KeywordDto>>(charm.Keywords.Select(x => x.Keyword)).Select(x =>
               {
                   x = x.GenerateLinks(_urlHelper, charmTypeId, id);
                   return x;
               });
            return Ok(keywords);
        }

        [HttpGet("{charmTypeId}/charms/{id}/keywords/{keywordId}", Name = "GetCharmKeyword")]
        public IActionResult GetKeyword(int charmTypeId, int id, int keywordId)
        {
            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }
            var charm = _repository.GetFirst<Charm>(x => x.Id == id && x.CharmTypeId == charmTypeId, null, "Keywords.Keyword");
            if (charm == null)
            {
                return NotFound();
            }

            if (charm.Keywords.All(x => x.KeywordId != keywordId))
            {
                return NotFound();
            }

            var keyword =
                Mapper.Map<KeywordDto>(charm.Keywords.Single(x => x.KeywordId == keywordId).Keyword);
            return Ok(keyword.GenerateLinks(_urlHelper, charmTypeId, id));
        }

        [HttpPut("{charmTypeId}/charms/{id}/keywords/{keywordId}", Name = "AddKeywordToCharm")]
        public IActionResult AddKeywordToCharm(int charmTypeId, int id, int keywordId)
        {
            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var charm = _repository.GetFirst<Charm>(x => x.Id == id && x.CharmTypeId == charmTypeId, null, "Keywords.Keyword");
            if (charm == null)
            {
                return NotFound();
            }

            var keyword = _repository.GetFirst<Keyword>(x => x.Id == keywordId);

            if (keyword == null)
            {
                return NotFound();
            }

            charm.AddKeyword(keyword);
            _repository.Update(charm);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

            return CreatedAtRoute("GetCharmKeyword", new { charmTypeId, id, keywordId },
                Mapper.Map<KeywordDto>(keyword).GenerateLinks(_urlHelper, charmTypeId, id));
        }

        [HttpDelete("{charmTypeId}/charms/{id}/keywords/{keywordId}", Name = "RemoveKeywordFromCharm")]
        public IActionResult RemoveKeyword(int charmTypeId, int id, int keywordId)
        {
            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var charm = _repository.GetFirst<Charm>(x => x.Id == id && x.CharmTypeId == charmTypeId, null, "Keywords.Keyword");
            if (charm == null)
            {
                return NotFound();
            }

            var keyword = charm.Keywords.SingleOrDefault(x => x.Keyword.Id == keywordId);

            if (keyword == null)
            {
                return NotFound();
            }

            charm.Keywords.Remove(keyword);
            _repository.Update(charm);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForCharm(int id, int charmTypeId, string fields)
        {
            var links = new List<LinkDto>
            {
                string.IsNullOrWhiteSpace(fields)
                    ? new LinkDto(_urlHelper.Link("GetCharmsForCharmType", new {id = id, charmTypeId = charmTypeId}),
                        "self", "GET")
                    : new LinkDto(
                        _urlHelper.Link("GetCharmsForCharmType",
                            new {id = id, charmTypeId = charmTypeId, fields = fields}), "self", "GET"),
                new LinkDto(_urlHelper.Link("DeleteCharm", new {charmTypeId = charmTypeId, id = id}), "delete_charm",
                    "DELETE"),
                new LinkDto(_urlHelper.Link("UpdateCharm", new {charmTypeId = charmTypeId, id = id}), "update_charm",
                    "PUT"),
                new LinkDto(_urlHelper.Link("PartiallyUpdateCharm", new {charmTypeId = charmTypeId, id = id}),
                    "partially_update_charm",
                    "PATCH"),
                new LinkDto(_urlHelper.Link("GetKeywordsForCharm", new {charmTypeId = charmTypeId, id = id}),
                    "get_keywords_for_charm", "GET")
            };

            return links;
        }
    }
}