using ExaltedCharm.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Extensions
{
    public static class DurationLinksExtensions
    {
        public  static DurationDto CreateLinksForDuration(this DurationDto duration, IUrlHelper urlHelper)
        {
            duration.Links.Add(new LinkDto(urlHelper.Link("GetDuration", new { id = duration.Id }), "self", "GET"));
            duration.Links.Add(new LinkDto(urlHelper.Link("GetCharmsForDuration", new { id = duration.Id }),
                "get-charms", "GET"));
            duration.Links.Add(new LinkDto(urlHelper.Link("DeleteDuration", new { id = duration.Id }), "delete_duration",
                "DELETE"));
            duration.Links.Add(new LinkDto(urlHelper.Link("UpdateDuration", new { id = duration.Id }), "update_duration",
                "PUT"));
            duration.Links.Add(new LinkDto(urlHelper.Link("PartiallyUpdateDuration", new { id = duration.Id }),
                "partially_update_duration",
                "PATCH"));
            return duration;
        }
    }
}