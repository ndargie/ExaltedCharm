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
    [Route("api/Abilities")]
    public class AbilityController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger<AbilityController> _logger;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;

        public AbilityController(IRepository repository, ILogger<AbilityController> logger, IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService, ITypeHelperService typeHelperService)
        {
            _repository = repository;
            _logger = logger;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetAbilities")]
        public IActionResult GetAbilities(AbilityResourceParameter abilityResourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<Ability, AbilityDto>
                (abilityResourceParameter.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.
                TypeHasProperties<AbilityDto>(abilityResourceParameter.Fields))
            {
                return BadRequest();
            }

            var abilities = _repository.GetAllOrderBy<Ability, AbilityDto>(abilityResourceParameter.OrderBy).AsQueryable();

            if (!string.IsNullOrWhiteSpace(abilityResourceParameter.Name))
            {
                var whereClause = abilityResourceParameter.Name.Trim().ToLowerInvariant();
                abilities = abilities.Where(x => x.Name.ToLowerInvariant() == whereClause);
            }

            var pagedList = PagedList<Ability>.Create(abilities, abilityResourceParameter.PageNumber,
                abilityResourceParameter.PageSize);

            var filters = string.IsNullOrWhiteSpace(abilityResourceParameter.Name)
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>()
                {
                    {
                        "Name", abilityResourceParameter.Name
                    }
                };
            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var pageMetsaData =
                    abilityResourceParameter.GeneratePagingMetaData(pagedList, "GetAbilities", _urlHelper);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));
                var links = abilityResourceParameter.GeneratePagingLinkData("GetAbilities", pagedList.HasNext,
                    pagedList.HasPrevious, _urlHelper, filters);
                var shapedAbilities = Mapper.Map<IEnumerable<AbilityDto>>(pagedList).ShapeData(abilityResourceParameter.Fields);
                var shapedAbilitiesWithLinks = shapedAbilities.Select(ability =>
                {
                    var abilitiesAsDictionary = ability as IDictionary<string, object>;
                    var abilitylinks =
                        CreateLinksForAbility((int)abilitiesAsDictionary["Id"], abilityResourceParameter.Fields);
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
                    abilityResourceParameter.GeneratePagingMetaData(pagedList, "GetAbilities", _urlHelper, filters,
                        pagedList.HasNext, pagedList.HasPrevious);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));
                return Ok(Mapper.Map<IEnumerable<AbilityDto>>(pagedList).ShapeData(abilityResourceParameter.Fields));
            }
        }

        [HttpGet("{id}", Name = "GetAbility")]
        public IActionResult GetAbility(int id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.
                TypeHasProperties<AbilityDto>(fields))
            {
                return BadRequest();
            }

            var ability = _repository.GetAll<Ability>().SingleOrDefault(x => x.Id == id);
            if (ability == null)
            {
                return NotFound();
            }

            var abilityDto = Mapper.Map<AbilityDto>(ability);

            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var links = CreateLinksForAbility(id, fields);

                var linkedResourceToReturn = abilityDto.ShapeData(fields) as IDictionary<string, object>;

                linkedResourceToReturn.Add("links", links);

                return Ok(linkedResourceToReturn);
            }
            return Ok(abilityDto.ShapeData(fields));
        }

        [HttpPost(Name = "CreateAbility")]
        public IActionResult CreateAbility([FromBody] AbilityForCreationDto ability,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (ability == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var abilityEntity = Mapper.Map<Ability>(ability);
            _repository.Create(abilityEntity);
            return !_repository.Save()
                ? StatusCode(500, "A problem happened while handling your request.")
                : CreatedAtRoute("GetAbility", new { id = abilityEntity.Id },
                    mediaType == "application/vnd.exalted.hateoas+json"
                        ? Mapper.Map<AbilityDto>(abilityEntity).GenerateLinks(_urlHelper)
                        : Mapper.Map<AbilityDto>(abilityEntity));
        }

        [HttpPost("{id}")]
        public IActionResult BlockAbilityCreation(int id)
        {
            var ability = _repository.GetAll<Ability>().SingleOrDefault(x => x.Id == id);
            return ability == null ? NotFound() : new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        [HttpPut("{id}", Name = "UpdateAbility")]
        public IActionResult UpdateAbility(int id, AbilityForUpdateDto ability)
        {
            if (ability == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var abilityEntity = _repository.GetFirst<Ability>(x => x.Id == id);
            if (abilityEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(ability, abilityEntity);

            _repository.Update(abilityEntity);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateAbility")]
        public IActionResult PatchAbility(int id, [FromBody] JsonPatchDocument<AbilityForUpdateDto> patchDoc,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var ability = _repository.GetAll<Ability>().SingleOrDefault(x => x.Id == id);
            if (ability == null)
            {
                var abilityDto = new AbilityForUpdateDto();
                patchDoc.ApplyTo(abilityDto, ModelState);

                TryValidateModel(abilityDto);

                if (!ModelState.IsValid)
                {
                    return new UnprocessableEntityObjectResult(ModelState);
                }

                var abilityToAdd = Mapper.Map<Ability>(abilityDto);
                abilityToAdd.Id = id;
                return !_repository.Save()
                    ? StatusCode(500, "A problem happened while handling your request.")
                    : CreatedAtRoute("GetDuration", new { id = abilityToAdd.Id },
                        mediaType == "application/vnd.exalted.hateoas+json"
                            ? Mapper.Map<AbilityDto>(abilityToAdd).GenerateLinks(_urlHelper)
                            : Mapper.Map<AbilityDto>(abilityToAdd));
            }

            var abilityToPatch = Mapper.Map<AbilityForUpdateDto>(ability);

            patchDoc.ApplyTo(abilityToPatch, ModelState);

            TryValidateModel(abilityToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(abilityToPatch, ability);

            _repository.Update(ability);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteAbility")]
        public IActionResult DeleteAbility(int id)
        {
            var ability = _repository.GetFirst<Ability>(x => x.Id == id);
            if (ability == null)
            {
                return NotFound();
            }

            _repository.Delete(ability);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForAbility(int id, string fields)
        {
            var links = new List<LinkDto>
            {
                string.IsNullOrWhiteSpace(fields)
                    ? new LinkDto(_urlHelper.Link("GetAbility", new {id}), "self", "GET")
                    : new LinkDto(_urlHelper.Link("GetAbility", new { id, fields}), "self", "GET"),
                new LinkDto(_urlHelper.Link("DeleteAbility", new {id}), "delete_ability",
                    "DELETE"),
                new LinkDto(_urlHelper.Link("UpdateAbility", new {id}), "update_ability",
                    "PUT"),
                new LinkDto(_urlHelper.Link("PartiallyUpdateAbility", new {id}),
                    "partially_update_ability",
                    "PATCH")
            };

            return links;
        }
    }
}