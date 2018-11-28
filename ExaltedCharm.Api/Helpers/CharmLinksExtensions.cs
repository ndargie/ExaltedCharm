using ExaltedCharm.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Helpers
{
    public static class CharmLinksExtensions
    {
        public static CharmDto GenerateLinks(this CharmDto charm, IUrlHelper urlHelper)
        {
            charm.Links.Add(new LinkDto(
                urlHelper.Link("GetCharm", new { charmTypeId = charm.CharmTypeId, id = charm.Id }), "self", "GET"));
            charm.Links.Add(new LinkDto(urlHelper.Link("UpdateCharm", new { charmTypeId = charm.CharmTypeId, id = charm.Id }),
                "update_charm", "PUT"));
            charm.Links.Add(new LinkDto(
                urlHelper.Link("PartiallyUpdateCharm", new { charmTypeId = charm.CharmTypeId, id = charm.Id }),
                "partially_update_charm", "PATCH"));
            charm.Links.Add(new LinkDto(
                urlHelper.Link("DeleteCharm", new { charmTypeId = charm.CharmTypeId, id = charm.Id }),
                "partially_update_charm", "PATCH"));
            charm.Links.Add(new LinkDto(
                urlHelper.Link("GetKeywordsForCharm", new { charmTypeId = charm.CharmTypeId, id = charm.Id }),
                "get_keywords", "GET"));
            return charm;
        }

        public static LinkedCollectionResourceWrapperDto<CharmDto> CreateDurationLinksForCharms(this 
            LinkedCollectionResourceWrapperDto<CharmDto> charmWrapper, IUrlHelper urlHelper)
        {
            charmWrapper.Links.Add(new LinkDto(urlHelper.Link("GetCharmsForDuration", new { }), "self", "GET"));
            return charmWrapper;
        }

        public static LinkedCollectionResourceWrapperDto<CharmDto> CreateCharmTypeLinksForCharms(this 
            LinkedCollectionResourceWrapperDto<CharmDto> charmWrapper, IUrlHelper urlHelper)
        {
            charmWrapper.Links.Add(new LinkDto(urlHelper.Link("GetCharmsForCharmType", new { }), "self", "GET"));
            return charmWrapper;
        }
    }
}