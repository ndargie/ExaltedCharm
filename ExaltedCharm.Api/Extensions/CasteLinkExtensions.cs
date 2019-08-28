using ExaltedCharm.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Extensions
{
    public static class CasteLinkExtensions
    {
        public static CasteDto GenerateLinks(this CasteDto caste, IUrlHelper urlHelper)
        {
            caste.Links.Add(new LinkDto(
                urlHelper.Link("GetCaste", new { exaltedTypeId = caste.ExaltedTypeId, id = caste.Id }), "self", "GET"));
            caste.Links.Add(new LinkDto(urlHelper.Link("UpdateCaste", new { exaltedTypeId = caste.ExaltedTypeId, id = caste.Id }),
                "update_caste", "PUT"));
            caste.Links.Add(new LinkDto(
                urlHelper.Link("PartiallyUpdateCaste", new { exaltedTypeId = caste.ExaltedTypeId, id = caste.Id }),
                "partially_update_caste", "PATCH"));
            caste.Links.Add(new LinkDto(
                urlHelper.Link("DeleteCaste", new { exaltedTypeId = caste.ExaltedTypeId, id = caste.Id }),
                "partially_update_caste", "PATCH"));
            return caste;
        }
    }
}