using ExaltedCharm.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Extensions
{
    public static class KeywordLinkEntension
    {
        public static KeywordDto GenerateLinks(this KeywordDto keyword, IUrlHelper urlHelper, int? charmTypeId = null, int? charmId = null)
        {
            if (charmTypeId.HasValue && charmId.HasValue)
            {
                keyword.Links.Add(new LinkDto(
                    urlHelper.Link("GetCharmKeyword",
                        new {id = charmId.Value, charmTypeId = charmTypeId.Value, keywordId = keyword.Id}), "self",
                    "GET"));
                keyword.Links.Add(new LinkDto(
                    urlHelper.Link("AddKeywordToCharm",
                        new {charmTypeId = charmTypeId.Value, id = charmId.Value, keywordId = "{KeywordId}"}), "add_keyword_to_charm",
                    "PUT"));
                keyword.Links.Add(new LinkDto(
                    urlHelper.Link("RemoveKeywordFromCharm",
                        new {charmTypeId = charmTypeId.Value, id = charmId.Value, keywordId = keyword.Id}),
                    "remove_keyword_from_charm",
                    "DELETE"));
            }
            else
            {
                keyword.Links.Add(new LinkDto(
                    urlHelper.Link("GetKeyword", new { id = keyword.Id }), "self", "GET"));
            }
            
            keyword.Links.Add(new LinkDto(
                urlHelper.Link("UpdateKeyword", new { id = keyword.Id }), "update_keyword", "PUT"));
            keyword.Links.Add(new LinkDto(
                urlHelper.Link("PartiallyUpdateKeyword", new {id = keyword.Id}), "partially_update_keyword", "PATCH"));
            keyword.Links.Add(new LinkDto(
                urlHelper.Link("DeleteKeyword", new { id = keyword.Id }), "delete_keyword", "DELETE"));
            return keyword;
        }
    }
}