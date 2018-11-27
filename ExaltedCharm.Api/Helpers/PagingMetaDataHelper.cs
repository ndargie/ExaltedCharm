using System.Collections.Generic;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Enums;
using ExaltedCharm.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Helpers
{
    public static class PagingMetaDataHelper
    {
        public static PagingMetaData GeneratePagingMetaData(this ResourceParameters durationResourceParameters, PagedList<Duration> pagedList, 
            string methodName, IUrlHelper urlHelper, Dictionary<string, string> filters)
        {
            if (filters == null)
            {
                filters = new Dictionary<string, string>();
            }
            var previousPageLink = pagedList.HasPrevious
                ? PagingUriGenerator.CreateResourceUri(urlHelper, methodName, durationResourceParameters,
                    ResourceUriType.PreviousPage, filters)
                : null;

            var nextPageLink = pagedList.HasPrevious
                ? PagingUriGenerator.CreateResourceUri(urlHelper, methodName, durationResourceParameters,
                    ResourceUriType.NextPage, filters)
                : null;

            var pageMetsaData = new PagingMetaData()
            {
                TotalCount = pagedList.TotalCount,
                PageSize = pagedList.PageSize,
                CurrentPage = pagedList.CurrentPage,
                TotalPages = pagedList.TotalPages,
                PreviousPageLink = previousPageLink,
                NextPageLink = nextPageLink
            };
            return pageMetsaData;
        }
    }
}