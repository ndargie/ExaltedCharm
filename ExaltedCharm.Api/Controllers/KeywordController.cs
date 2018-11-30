using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Helpers;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExaltedCharm.Api.Controllers
{
    [Route("api/keywords")]
    public class KeywordController : Controller
    {
        private readonly IRepository _repository;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;

        public KeywordController(IRepository repository, IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _repository = repository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetKeywords")]
        public IActionResult GetKeywords(KeywordResourceParameter keywordResourceParameter, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<Keyword, KeywordDto>
                (keywordResourceParameter.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<KeywordDto>(keywordResourceParameter.Fields))
            {
                return BadRequest();
            }

            var keywords = _repository
                .GetAllOrderBy<Keyword, KeywordDto>(keywordResourceParameter.OrderBy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keywordResourceParameter.Name))
            {
                var whereClause = keywordResourceParameter.Name.Trim().ToLowerInvariant();
                keywords = keywords.Where(x => x.Name.ToLowerInvariant() == whereClause);
            }

            var pagedList = PagedList<Keyword>.Create(keywords, keywordResourceParameter.PageNumber,
                keywordResourceParameter.PageSize);

            var filters = string.IsNullOrWhiteSpace(keywordResourceParameter.Name)
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>()
                {
                    {
                        "Name", keywordResourceParameter.Name
                    }
                };

            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var pageMetsaData =
                    keywordResourceParameter.GeneratePagingMetaData(pagedList, "GetKeywords", _urlHelper);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));

                var links = keywordResourceParameter.GeneratePagingLinkData("GetKeywords", pagedList.HasNext,
                    pagedList.HasPrevious, _urlHelper, filters);
                var shapedKeyword = Mapper.Map<IEnumerable<KeywordDto>>(pagedList).ShapeData(keywordResourceParameter.Fields);

                var shapedKeywordsWithLinks = shapedKeyword.Select(keyword =>
                {
                    var keywordAsDictionary = keyword as IDictionary<string, object>;
                    var keywordLink =
                        CreateLinksForKeyword((int)keywordAsDictionary["Id"], keywordResourceParameter.Fields);
                    keywordAsDictionary.Add("links", keywordLink);
                    return keywordAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedKeywordsWithLinks,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var pageMetsaData =
                    keywordResourceParameter.GeneratePagingMetaData(pagedList, "GetKeywords", _urlHelper, filters,
                        pagedList.HasNext, pagedList.HasPrevious);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));
                return Ok(Mapper.Map<IEnumerable<KeywordDto>>(pagedList).ShapeData(keywordResourceParameter.Fields));
            }
        }

        [HttpGet("{id}", Name = "GetKeyword")]
        public IActionResult GetKeyword(int id,
            [FromQuery]string fields, 
            [FromHeader(Name = "Accept")] string mediaType)
        {

            if (!_typeHelperService.
                TypeHasProperties<KeywordDto>(fields))
            {
                return BadRequest();
            }

            var keyword = _repository.GetFirst<Keyword>(x => x.Id == id);
            if (keyword == null)
            {
                return NotFound();
            }

            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var links = CreateLinksForKeyword(id, fields);

                var linkedResourceToReturn = keyword.ShapeData(fields) as IDictionary<string, object>;

                linkedResourceToReturn.Add("links", links);

                return Ok(linkedResourceToReturn);
            }

            return Ok(Mapper.Map<KeywordDto>(keyword).ShapeData(fields));
        }

        [HttpPost(Name = "CreateKeyword")]
        public IActionResult CreateKeyword([FromBody] KeywordCreationDto keyword, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (keyword == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var finalKeyword = Mapper.Map<Keyword>(keyword);

            _repository.Create(finalKeyword);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return CreatedAtRoute("GetKeyword", new {id = finalKeyword.Id},
                mediaType == "application/vnd.exalted.hateoas+json"
                    ? Mapper.Map<KeywordDto>(finalKeyword).GenerateLinks(_urlHelper)
                    : Mapper.Map<KeywordDto>(finalKeyword));
        }

        [HttpPut("{id}", Name = "UpdateKeyword")]
        public IActionResult UpdateKeyword(int id,
            [FromBody] KeywordUpdateDto keyword)
        {
            if (keyword == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_repository.GetExists<Keyword>(x => x.Id == id))
            {
                return NotFound();
            }

            var keywordEntity = _repository.GetFirst<Keyword>(x => x.Id == id);
            Mapper.Map(keyword, keywordEntity);
            _repository.Update(keywordEntity);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateKeyword")]
        public IActionResult PatchKeyword(int id,
            [FromBody] JsonPatchDocument<KeywordUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var keywordEntity = _repository.GetFirst<Keyword>(x => x.Id == id);
            if (keywordEntity == null)
            {
                return NotFound();
            }

            var keywordToPatch = AutoMapper.Mapper.Map<KeywordUpdateDto>(keywordEntity);
            patchDocument.ApplyTo(keywordToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TryValidateModel(keywordToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            AutoMapper.Mapper.Map(keywordToPatch, keywordEntity);
            _repository.Update(keywordEntity);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteKeyword")]
        public IActionResult DeleteKeyword(int id)
        {
            var keyword = _repository.GetAll<Keyword>().SingleOrDefault(x => x.Id == id);
            if (keyword == null)
            {
                return NotFound();
            }

            _repository.Delete(keyword);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForKeyword(int id, string fields)
        {
            var links = new List<LinkDto>
            {
                string.IsNullOrWhiteSpace(fields)
                    ? new LinkDto(_urlHelper.Link("GetKeyword", new {id = id}), "self", "GET")
                    : new LinkDto(_urlHelper.Link("GetKeyword", new {id = id, fields = fields}), "self", "GET"),
                new LinkDto(_urlHelper.Link("DeleteKeyword", new {id = id}), "delete_keyword",
                    "DELETE"),
                new LinkDto(_urlHelper.Link("UpdateKeyword", new {id = id}), "update_keyword",
                    "PUT"),
                new LinkDto(_urlHelper.Link("PartiallyUpdateKeyword", new {id = id}),
                    "partially_update_keyword",
                    "PATCH")
            };

            return links;
        }
    }
}