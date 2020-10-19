using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Extensions;
using ExaltedCharm.Api.Helpers;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ExaltedCharm.Api.Controllers
{
    [Route("api/Weapon")]
    public class WeaponController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger<WeaponController> _logger;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;

        public WeaponController(IRepository repository, ILogger<WeaponController> logger, IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService, ITypeHelperService typeHelperService)
        {
            _repository = repository;
            _logger = logger;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetRangedWeapons")]
        public IActionResult GetRangedWeapons(RangedWeaponResourceParameters rangedWeaponResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<RangedWeapon, RangedWeaponDto>
                (rangedWeaponResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.
                TypeHasProperties<RangedWeaponDto>(rangedWeaponResourceParameters.Fields))
            {
                return BadRequest();
            }

            var rangedWeapons = _repository.GetAllOrderBy<RangedWeapon, RangedWeaponDto>(rangedWeaponResourceParameters.OrderBy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(rangedWeaponResourceParameters.Name))
            {
                var whereClause = rangedWeaponResourceParameters.Name.Trim().ToLowerInvariant();
                rangedWeapons = rangedWeapons.Where(x => x.Name.ToLowerInvariant() == whereClause);
            }

            var pagedList = PagedList<RangedWeapon>.Create(rangedWeapons, rangedWeaponResourceParameters.PageNumber,
                rangedWeaponResourceParameters.PageSize);

            var filters = string.IsNullOrWhiteSpace(rangedWeaponResourceParameters.Name)
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>()
                {
                    {
                        "Name", rangedWeaponResourceParameters.Name
                    }
                };

            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var pageMetsaData =
                    rangedWeaponResourceParameters.GeneratePagingMetaData(pagedList, "GetRangedWeapons", _urlHelper);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));
                var links = rangedWeaponResourceParameters.GeneratePagingLinkData("GetRangedWeapons", pagedList.HasNext,
                    pagedList.HasPrevious, _urlHelper, filters);
                var shappedRangedWeapons = Mapper.Map<IEnumerable<RangedWeaponDto>>(pagedList).ShapeData(rangedWeaponResourceParameters.Fields);
                var shappedRangedWeaponsWithLinks = shappedRangedWeapons.Select(rangedWeapon =>
                {
                    var rangedWeaponsAsDictionary = rangedWeapon as IDictionary<string, object>;
                    var rangedWeaponsLinks =
                        CreateLinksForWeaponTags((int)rangedWeaponsAsDictionary["Id"], rangedWeaponResourceParameters.Fields);
                    rangedWeaponsAsDictionary.Add("links", rangedWeaponsLinks);
                    return rangedWeaponsAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shappedRangedWeaponsWithLinks,
                    links
                };

                return Ok(linkedCollectionResource);

            }
            else
            {
                var pageMetsaData =
                    rangedWeaponResourceParameters.GeneratePagingMetaData(pagedList, "GetRangedWeapons", _urlHelper, filters,
                        pagedList.HasNext, pagedList.HasPrevious);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageMetsaData));
                return Ok(Mapper.Map<IEnumerable<RangedWeaponDto>>(pagedList).ShapeData(rangedWeaponResourceParameters.Fields));
            }
        }

        private IEnumerable<LinkDto> CreateLinksForWeaponTags(int id, string fields)
        {
            var links = new List<LinkDto>
            {
                string.IsNullOrWhiteSpace(fields)
                    ? new LinkDto(_urlHelper.Link("GetRangedWeapons", new {id}), "self", "GET")
                    : new LinkDto(_urlHelper.Link("GetRangedWeapons", new { id, fields}), "self", "GET")
            };

            return links;
        }
    }
}