using System.Collections.Generic;
using ExaltedCharm.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Controllers
{
    [Route("api")]
    public class RootController : Controller
    {
        private readonly IUrlHelper _urlHelper;

        public RootController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
        {
            if (mediaType == "application/vnd.exalted.hateoas+json")
            {
                var links = new List<LinkDto>
                {
                    new LinkDto(_urlHelper.Link("GetRoot", new { }), "self", "GET"),
                    new LinkDto(_urlHelper.Link("GetExaltedTypes", new { }), "exaltedtypes", "GET"),
                    new LinkDto(_urlHelper.Link("CreateExaltedType", new { }), "create_exaltedtype", "POST"),
                    new LinkDto(_urlHelper.Link("GetCharmTypes", new { }), "charmtypes", "GET"),
                    new LinkDto(_urlHelper.Link("CreateCharmType", new { }), "create_charmtype", "POST"),
                    new LinkDto(_urlHelper.Link("GetAbilities", new { }), "abilities", "GET"),
                    new LinkDto(_urlHelper.Link("CreateAbility", new { }), "create_ability", "POST"),
                    new LinkDto(_urlHelper.Link("GetDurations", new { }), "durations", "GET"),
                    new LinkDto(_urlHelper.Link("CreateDuration", new { }), "create_duration", "POST"),
                    new LinkDto(_urlHelper.Link("GetKeywords", new { }), "keywords", "GET"),
                    new LinkDto(_urlHelper.Link("CreateKeyword", new { }), "create_keyword", "POST")
                };
                return Ok(links);

            }

            return NoContent();
        }
    }
}
