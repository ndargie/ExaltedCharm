using ExaltedCharm.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Helpers
{
    public static class CharmTypeLinkExtensions
    {
        public static CharmTypeDto GenerateLinks(this CharmTypeDto charmTypeDto, IUrlHelper urlHelper)
        {
            charmTypeDto.Links.Add(new LinkDto(
                urlHelper.Link("GetCharmType", new {id = charmTypeDto.Id, includeCharms = true}), "self", "GET"));
            charmTypeDto.Links.Add(new LinkDto(urlHelper.Link("DeleteCharmType", new {id = charmTypeDto.Id}),
                "delete_charmtype", "DELETE"));
            charmTypeDto.Links.Add(new LinkDto(urlHelper.Link("UpdateCharmType", new {id = charmTypeDto.Id}),
                "update_charmtype", "PUT"));
            charmTypeDto.Links.Add(new LinkDto(urlHelper.Link("PartiallyUpdateCharmType", new { id = charmTypeDto.Id }),
                "partially_update_charmtype", "PATCH"));
            charmTypeDto.Links.Add(new LinkDto(
                urlHelper.Link("GetCharmsForCharmType", new {charmTypeId = charmTypeDto.Id}), "get_charms", "GET"));
            return charmTypeDto;
        }

        public static CharmTypeWithoutCharmsDto GenerateLinks(this CharmTypeWithoutCharmsDto charmTypeWithoutCharmsDto,
            IUrlHelper urlHelper)
        {
            charmTypeWithoutCharmsDto.Links.Add(new LinkDto(
                urlHelper.Link("GetCharmType", new { id = charmTypeWithoutCharmsDto.Id}), "self", "GET"));
            charmTypeWithoutCharmsDto.Links.Add(new LinkDto(urlHelper.Link("DeleteCharmType", new { id = charmTypeWithoutCharmsDto.Id }),
                "delete_charmtype", "DELETE"));
            charmTypeWithoutCharmsDto.Links.Add(new LinkDto(urlHelper.Link("UpdateCharmType", new { id = charmTypeWithoutCharmsDto.Id }),
                "update_charmtype", "PUT"));
            charmTypeWithoutCharmsDto.Links.Add(new LinkDto(urlHelper.Link("PartiallyUpdateCharmType", new { id = charmTypeWithoutCharmsDto.Id }),
                "partially_update_charmtype", "PATCH"));
            charmTypeWithoutCharmsDto.Links.Add(new LinkDto(
                urlHelper.Link("GetCharmsForCharmType", new { charmTypeId = charmTypeWithoutCharmsDto.Id }), "get_charms", "GET"));
            return charmTypeWithoutCharmsDto;
        }
    }
}