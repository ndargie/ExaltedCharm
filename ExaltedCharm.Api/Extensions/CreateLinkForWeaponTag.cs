using ExaltedCharm.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Extensions
{
    public static class CreateLinkForWeaponTag
    {
        public static WeaponTagDto CreateLinksForWeaponTag(this WeaponTagDto weaponTag, IUrlHelper urlHelper)
        {
            weaponTag.Links.Add(new LinkDto(urlHelper.Link("GetDuration", new { id = weaponTag.Id }), "self", "GET"));
            weaponTag.Links.Add(new LinkDto(urlHelper.Link("DeleteWeaponTag", new { id = weaponTag.Id }), "delete_weapontag",
                "DELETE"));
            weaponTag.Links.Add(new LinkDto(urlHelper.Link("UpdateWeaponTag", new { id = weaponTag.Id }), "update_weapontag",
                "PUT"));
            weaponTag.Links.Add(new LinkDto(urlHelper.Link("PartiallyUpdateWeaponTag", new { id = weaponTag.Id }),
                "partially_update_weapontag",
                "PATCH"));
            return weaponTag;
        }
    }
}