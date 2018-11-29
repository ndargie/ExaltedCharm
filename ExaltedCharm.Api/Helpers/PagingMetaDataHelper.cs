using System.Collections.Generic;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Enums;
using ExaltedCharm.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Helpers
{
    public static class PagingMetaDataHelper
    {
        public static PagingMetaData  GeneratePagingMetaData<T>(this ResourceParameters resourceParameters, PagedList<T> pagedList, 
            string methodName, IUrlHelper urlHelper, Dictionary<string, string> filters = null, bool addNextPage = false, bool addPreviousPage = false)
        {
            if (filters == null)
            {
                filters = new Dictionary<string, string>();
            }
            var pageMetsaData = new PagingMetaData()
            {
                TotalCount = pagedList.TotalCount,
                PageSize = pagedList.PageSize,
                CurrentPage = pagedList.CurrentPage,
                TotalPages = pagedList.TotalPages,
                NextPage = addNextPage && pagedList.HasNext
                    ? PagingUriGenerator.CreateResourceUri(urlHelper, methodName, resourceParameters,
                        ResourceUriType.NextPage, filters)
                    : null,
                PreviousPage = addPreviousPage && pagedList.HasPrevious
                    ? PagingUriGenerator.CreateResourceUri(urlHelper, methodName, resourceParameters,
                        ResourceUriType.PreviousPage, filters)
                    : null
            };
            return pageMetsaData;
        }

        public static List<LinkDto> GeneratePagingLinkData(this ResourceParameters resourceParameters,
            string methodName, bool hasNext,
            bool hasPrevious, IUrlHelper urlHelper, Dictionary<string, string> filters)
        {
            if (filters == null)
            {
                filters = new Dictionary<string, string>();
            }

            var links = new List<LinkDto>()
            {
                new LinkDto(
                    PagingUriGenerator.CreateResourceUri(urlHelper, methodName, resourceParameters,
                        ResourceUriType.Current, filters), "self", "GET"),
            };
            if (hasNext)
            {
                links.Add(new LinkDto(
                    PagingUriGenerator.CreateResourceUri(urlHelper, methodName, resourceParameters,
                        ResourceUriType.NextPage, filters), "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(
                    PagingUriGenerator.CreateResourceUri(urlHelper, methodName, resourceParameters,
                        ResourceUriType.PreviousPage, filters), "previousPage", "GET"));
            }

            return links;
        }
        
    }
}