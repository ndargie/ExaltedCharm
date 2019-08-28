using ExaltedCharm.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Extensions
{
    public static class AbilityLinkExtensions
    {
        public static AbilityDto GenerateLinks(this AbilityDto ability, IUrlHelper urlHelper, int? exaltedTypeId = null,
            int? casteId = null)
        {
            if (exaltedTypeId.HasValue && casteId.HasValue)
            {
                ability.Links.Add(new LinkDto(
                    urlHelper.Link("GetCasteAbility",
                        new { exaltedTypeId = exaltedTypeId.Value, id = casteId.Value, abilityId = ability.Id}),
                    "self", "GET"));
                ability.Links.Add(new LinkDto(
                    urlHelper.Link("AddAbilityToCaste",
                        new {exaltedTypeId = exaltedTypeId.Value, id = casteId.Value, abilityId = "AbilityId"}),
                    "add_ability_to_caste", "PUT"));
                ability.Links.Add(new LinkDto(
                    urlHelper.Link("RemoveAbilityFromCaste",
                        new {exaltedTypeId = exaltedTypeId.Value, id = casteId.Value, abilityId = ability.Id}),
                    "remove_ability_from_charm", "DELETE"));
            }
            else
            {
                ability.Links.Add(new LinkDto(urlHelper.Link("GetAbility", new {id = ability.Id}), "self", "GET"));
                ability.Links.Add(new LinkDto(urlHelper.Link("DeleteAbility", new {id = ability.Id}), "delete_ability",
                    "DELETE"));
                ability.Links.Add(new LinkDto(urlHelper.Link("UpdateAbility", new {id = ability.Id}), "update_ability",
                    "PUT"));
                ability.Links.Add(new LinkDto(urlHelper.Link("PartiallyUpdateAbility", new {id = ability.Id}),
                    "partially_update_ability",
                    "PATCH"));
            }
            return ability;
        }

        public static AbilityDto GenerateLinks(this AbilityDto ability, IUrlHelper urlHelper, int charmTypeId,
            int charmId)
        {
            ability.Links.Add(new LinkDto(
                urlHelper.Link("GetAbilityForCharm", new {charmTypeId = charmTypeId, id = charmId}), "self", "GET"));
            ability.Links.Add(new LinkDto(
                urlHelper.Link("SetAbilityForCharm", new {charmTypeId, id = charmTypeId, abilityId = "{abilityId}"}),
                "add_ability_to_charm", "PUT"));
            return ability;
        }
    }
}