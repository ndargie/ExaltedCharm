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
    [Route("api/exaltedTypes")]
    public class CasteController : Controller
    {
        private readonly ILogger<CasteController> _logger;
        private readonly IRepository _repository;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;

        public CasteController(ILogger<CasteController> logger, IRepository repository, IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService, ITypeHelperService typeHelperService)
        {
            _logger = logger;
            _repository = repository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet("{exaltedTypeId}/castes", Name = "GetCastesForExaltedType")]
        public IActionResult GetCastes(int exaltedTypeId, CasteResourceParameter casteResourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<Caste, CasteDto>
                (casteResourceParameter.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<CharmDto>(casteResourceParameter.Fields))
            {
                return BadRequest();
            }

            try
            {
                if (!_repository.GetExists<ExaltedType>(x => x.Id == exaltedTypeId))
                {
                    _logger.LogInformation($"Charmtype with id {exaltedTypeId} wasn't found when accessing charms");
                    return NotFound();
                }

                var castes = _repository.GetAllOrderBy<Caste, CasteDto>(casteResourceParameter.OrderBy)
                    .Where(x => x.ExaltedTypeId == exaltedTypeId).AsQueryable();

                if (!string.IsNullOrWhiteSpace(casteResourceParameter.Name))
                {
                    var whereClause = casteResourceParameter.Name.Trim().ToLowerInvariant();
                    castes = castes.Where(x => x.Name.ToLowerInvariant() == whereClause);
                }

                var pagedList = PagedList<Caste>.Create(castes, casteResourceParameter.PageNumber,
                    casteResourceParameter.PageSize);
                var filters = string.IsNullOrWhiteSpace(casteResourceParameter.Name)
                    ? new Dictionary<string, string>()
                    : new Dictionary<string, string>()
                    {
                        {
                            "Name", casteResourceParameter.Name
                        }
                    };

                if (mediaType == "application/vnd.exalted.hateoas+json")
                {
                    var pageMetsaData =
                        casteResourceParameter.GeneratePagingMetaData(pagedList, "GetCastesForExaltedType", _urlHelper, filters);
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));
                    var links = casteResourceParameter.GeneratePagingLinkData("GetCastesForExaltedType", pagedList.HasNext,
                        pagedList.HasPrevious, _urlHelper, filters);
                    var shapedCaste = Mapper.Map<IEnumerable<CasteDto>>(pagedList).ShapeData(casteResourceParameter.Fields);
                    var shapedCasteWithLinks = shapedCaste.Select(charm =>
                    {
                        var casteAsDictionary = charm as IDictionary<string, object>;
                        var charmLink =
                            CreateLinksForCaste((int)casteAsDictionary["Id"], exaltedTypeId, casteResourceParameter.Fields);
                        casteAsDictionary.Add("links", charmLink);
                        return casteAsDictionary;
                    });

                    var linkedCollectionResource = new
                    {
                        value = shapedCasteWithLinks,
                        links
                    };

                    return Ok(linkedCollectionResource);
                }
                else
                {
                    var pageMetsaData =
                        casteResourceParameter.GeneratePagingMetaData(pagedList, "GetCastesForExaltedType", _urlHelper, filters,
                            pagedList.HasNext, pagedList.HasPrevious);
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));

                    return Ok(Mapper.Map<IEnumerable<CasteDto>>(pagedList).ShapeData(casteResourceParameter.Fields));
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting castes for exaltedtype {exaltedTypeId}", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{exaltedTypeId}/castes/{id}", Name = "GetCaste")]
        public IActionResult GetCaste(int exaltedTypeId, int id, string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.
                TypeHasProperties<KeywordDto>(fields))
            {
                return BadRequest();
            }

            if (!_repository.GetExists<ExaltedType>(x => x.Id == exaltedTypeId))
            {
                _logger.LogInformation($"ExaltedType with id {exaltedTypeId} wasn't found when accessing castes");
                return NotFound();
            }

            var caste = _repository.GetAll<Caste>().SingleOrDefault(x => x.ExaltedTypeId == exaltedTypeId && x.Id == id);
            if (caste == null)
            {
                return NotFound();
            }

            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var links = CreateLinksForCaste(id, exaltedTypeId, fields);
                var linkedResourceToReturn = Mapper.Map<CasteDto>(caste).ShapeData(fields) as IDictionary<string, object>;

                linkedResourceToReturn.Add("links", links);

                return Ok(linkedResourceToReturn);
            }

            return Ok(Mapper.Map<CasteDto>(caste).ShapeData(fields));
        }

        [HttpPost("{exaltedTypeId}/castes")]
        public IActionResult CreateCaste(int exaltedTypeId, [FromBody] CasteForCreationDto caste)
        {
            if (caste == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_repository.GetExists<ExaltedType>(x => x.Id == exaltedTypeId))
            {
                return NotFound();
            }

            var finalCaste = Mapper.Map<Caste>(caste);

            var exaltedType = _repository.GetFirst<ExaltedType>(x => x.Id == exaltedTypeId);

            exaltedType.Castes.Add(finalCaste);

            if (!_repository.Save())
            {
                StatusCode(500, "A problem happened while handling your request.");
            }

            return CreatedAtRoute("GetCharm", new { exaltedTypeId, id = finalCaste.Id },
                Mapper.Map<CasteDto>(finalCaste).GenerateLinks(_urlHelper));
        }

        [HttpPut("{exaltedTypeId}/castes/{id}", Name = "UpdateCaste")]
        public IActionResult UpdateCaste(int exaltedTypeId, int id, [FromBody] CasteForUpdateDto caste)
        {
            if (caste == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_repository.GetExists<ExaltedType>(x => x.Id == exaltedTypeId))
            {
                return NotFound();
            }

            var casteEntity = _repository.GetAll<Caste>()
                .SingleOrDefault(x => x.ExaltedTypeId == exaltedTypeId && x.Id == id);
            if (casteEntity == null)
            {
                var casteToAdd = Mapper.Map<Caste>(caste);
                casteToAdd.Id = id;
                var exaltedType = _repository.GetAll<ExaltedType>().Single(x => x.Id == exaltedTypeId);
                exaltedType.Castes.Add(casteToAdd);
                return _repository.Save()
                    ? StatusCode(500, "A problem happend while handling your request")
                    : CreatedAtRoute("GetCharm", new { exaltedTypeId, id = casteToAdd.Id },
                        Mapper.Map<CasteDto>(casteToAdd).GenerateLinks(_urlHelper));
            }

            Mapper.Map(caste, casteEntity);
            _repository.Update(casteEntity);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happend while handling your request");
            }

            return NoContent();
        }

        [HttpPatch("{exaltedTypeId}/castes/{id}", Name = "PartiallyUpdateCaste")]
        public IActionResult PartiallyUpdateCaste(int exaltedTypeId, int id,
            [FromBody] JsonPatchDocument<CasteForUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_repository.GetExists<ExaltedType>(x => x.Id == exaltedTypeId))
            {
                return NotFound();
            }

            var casteEntity = _repository.GetAll<Caste>().SingleOrDefault(x => x.ExaltedTypeId == exaltedTypeId && x.Id == id);
            if (casteEntity == null)
            {
                return NotFound();
            }

            var casteToPatch = Mapper.Map<CasteForUpdateDto>(casteEntity);

            patchDocument.ApplyTo(casteToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TryValidateModel(casteToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(casteToPatch, casteEntity);
            _repository.Update(casteEntity);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

            return NoContent();
        }

        [HttpDelete("{exaltedTypeId}/castes/{id}", Name = "DeleteCaste")]
        public IActionResult DeleteCaste(int exaltedTypeId, int id)
        {
            if (!_repository.GetExists<ExaltedType>(x => x.Id == exaltedTypeId))
            {
                return NotFound();
            }

            var casteEntity = _repository.GetAll<Caste>().SingleOrDefault(x => x.ExaltedTypeId == exaltedTypeId && x.Id == id);
            if (casteEntity == null)
            {
                return NotFound();
            }

            _repository.Delete(casteEntity);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

            return NoContent();
        }

        [HttpGet("{exaltedTypeId}/castes/{id}/abilities", Name = "GetAbilitiesForCaste")]
        public IActionResult GetAbilities(int exaltedTypeId, int id)
        {
            if (!_repository.GetExists<ExaltedType>(x => x.Id == exaltedTypeId))
            {
                return NotFound();
            }

            var caste = _repository.GetFirst<Caste>(x => x.Id == id && x.ExaltedTypeId == exaltedTypeId, null,
                "Abilities.Ability");
            if (caste == null)
            {
                return NotFound();
            }

            var abilities = Mapper.Map<IEnumerable<AbilityDto>>(caste.Abilities.Select(x => x.Ability)).Select(x =>
            {
                x = x.GenerateLinks(_urlHelper, exaltedTypeId, id);
                return x;
            });
            return Ok(abilities);

        }

        [HttpGet("{exaltedTypeId}/castes/{id}/abilities/{abilityId}", Name = "GetCasteAbility")]
        public IActionResult GetAbility(int exaltedTypeId, int id, int abilityId)
        {
            if (!_repository.GetExists<ExaltedType>(x => x.Id == exaltedTypeId))
            {
                return NotFound();
            }
            var caste = _repository.GetFirst<Caste>(x => x.Id == id && x.ExaltedTypeId == exaltedTypeId, null, "Abilities.Ability");
            if (caste == null)
            {
                return NotFound();
            }

            if (caste.Abilities.All(x => x.AbilityId != abilityId))
            {
                return NotFound();
            }

            var ability =
                Mapper.Map<AbilityDto>(caste.Abilities.Single(x => x.AbilityId == abilityId).Ability);
            return Ok(ability.GenerateLinks(_urlHelper, exaltedTypeId, id));
        }

        [HttpPut("{exaltedTypeId}/castes/{id}/abilities/{abilityId}", Name = "AddAbilityToCaste")]
        public IActionResult AddAbility(int exaltedTypeId, int id, int abilityId)
        {
            if (!_repository.GetExists<ExaltedType>(x => x.Id == exaltedTypeId))
            {
                return NotFound();
            }

            var caste = _repository.GetFirst<Caste>(x => x.Id == id && x.ExaltedTypeId == exaltedTypeId, null,
                "Abilities.Ability");
            if (caste == null)
            {
                return NotFound();
            }

            var ability = _repository.GetFirst<Ability>(x => x.Id == abilityId);

            if (ability == null)
            {
                return NotFound();
            }

            if (caste.Abilities.All(x => x.AbilityId != abilityId))
            {
                caste.Abilities.Add(new CasteAbility() {Ability = ability, Caste = caste});
                _repository.Update(caste);
                if (!_repository.Save())
                {
                    return StatusCode(500, "A problem happened while handling your request");
                }
            }
            return CreatedAtRoute("GetCasteAbility", new { exaltedTypeId, id, abilityId },
                Mapper.Map<AbilityDto>(ability).GenerateLinks(_urlHelper, exaltedTypeId, id));
        }

        [HttpDelete]
        [HttpPut("{exaltedTypeId}/castes/{id}/abilities/{abilityId}", Name = "RemoveAbilityFromCaste")]
        public IActionResult RemoveAbility(int exaltedTypeId, int id, int abilityId)
        {
            if (!_repository.GetExists<ExaltedType>(x => x.Id == exaltedTypeId))
            {
                return NotFound();
            }

            var caste = _repository.GetFirst<Caste>(x => x.Id == id && x.ExaltedTypeId == exaltedTypeId, null,
                "Abilities.Ability");
            if (caste == null)
            {
                return NotFound();
            }

            var ability = caste.Abilities.SingleOrDefault(x => x.AbilityId == abilityId);

            if (ability == null)
            {
                return NotFound();
            }

            caste.Abilities.Remove(ability);
            _repository.Update(caste);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForCaste(int id, int exaltedTypeId, string fields)
        {
            var links = new List<LinkDto>
            {
                string.IsNullOrWhiteSpace(fields)
                    ? new LinkDto(_urlHelper.Link("GetCaste", new {id = id, exaltedTypeId = exaltedTypeId}),
                        "self", "GET")
                    : new LinkDto(
                        _urlHelper.Link("GetCaste",
                            new {id = id, exaltedTypeId = exaltedTypeId, fields = fields}), "self", "GET"),
                new LinkDto(_urlHelper.Link("GetAbilitiesForCaste", new {casteId = id}),
                    "get_abilities", "GET"),
                new LinkDto(_urlHelper.Link("DeleteCaste", new {exaltedTypeId = exaltedTypeId, id = id}),
                    "delete_caste",
                    "DELETE"),
                new LinkDto(_urlHelper.Link("UpdateCaste", new {exaltedTypeId = exaltedTypeId, id = id}),
                    "update_caste",
                    "PUT"),
                new LinkDto(_urlHelper.Link("PartiallyUpdateCaste", new {exaltedTypeId = exaltedTypeId, id = id}),
                    "partially_update_caste",
                    "PATCH")
            };

            return links;
        }
    }
}