using ExaltedCharm.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Helpers
{
    public static class ExaltedTypeLinkExtensions
    {
        public static ExaltedTypeWithoutCastesDto GenerateLinks(this ExaltedTypeWithoutCastesDto exaltedTypeDto, IUrlHelper urlHelper)
        {
            exaltedTypeDto.Links.Add(new LinkDto(urlHelper.Link("GetExaltedType", new {Id = exaltedTypeDto.Id}), "self",
                "GET"));
            exaltedTypeDto.Links.Add(new LinkDto(urlHelper.Link("DeleteExaltedType", new {id = exaltedTypeDto.Id}),
                "delete_exaltedtype", "DELETE"));
            exaltedTypeDto.Links.Add(new LinkDto(urlHelper.Link("UpdateExaltedType", new { id = exaltedTypeDto.Id }),
                "update_exaltedtype", "PUT"));
            exaltedTypeDto.Links.Add(new LinkDto(urlHelper.Link("PartiallyUpdateExaltedType", new { id = exaltedTypeDto.Id }),
                "partially_update_exaltedtype", "PATCH"));
            return exaltedTypeDto;
        }

        public static ExaltedTypeDto GenerateLinks(this ExaltedTypeDto exaltedTypeDto, IUrlHelper urlHelper)
        {
            exaltedTypeDto.Links.Add(new LinkDto(urlHelper.Link("GetExaltedType", new { Id = exaltedTypeDto.Id, includeCastes = true }), "self",
                "GET"));
            exaltedTypeDto.Links.Add(new LinkDto(urlHelper.Link("DeleteExaltedType", new { id = exaltedTypeDto.Id }),
                "delete_exaltedtype", "DELETE"));
            exaltedTypeDto.Links.Add(new LinkDto(urlHelper.Link("UpdateExaltedType", new { id = exaltedTypeDto.Id }),
                "update_exaltedtype", "PUT"));
            exaltedTypeDto.Links.Add(new LinkDto(urlHelper.Link("PartiallyUpdateExaltedType", new { id = exaltedTypeDto.Id }),
                "partially_update_exaltedtype", "PATCH"));
            return exaltedTypeDto;
        }
    }
}