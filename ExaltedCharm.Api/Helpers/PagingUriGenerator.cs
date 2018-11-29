using System.Collections.Generic;
using ExaltedCharm.Api.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Helpers
{
    public static class PagingUriGenerator
    {
        public static string CreateResourceUri(IUrlHelper urlHelper, string methodName, 
            ResourceParameters resourceParameters, 
            ResourceUriType type,
            Dictionary<string, string> filters)
        {

            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link(methodName, new
                    {
                        searchCriteria = resourceParameters.SearchCriteria,
                        pageNumber = resourceParameters.PageNumber - 1,
                        pageSize = resourceParameters.PageSize,
                        filters,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields
                    });
                case ResourceUriType.NextPage:
                    return urlHelper.Link(methodName, new
                    {
                        searchCriteria = resourceParameters.SearchCriteria,
                        pageNumber = resourceParameters.PageNumber + 1,
                        pageSize = resourceParameters.PageSize,
                        filters,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields
                    });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link(methodName, new
                    {
                        searchCriteria = resourceParameters.SearchCriteria,
                        pageNumber = resourceParameters.PageNumber,
                        pageSize = resourceParameters.PageSize,
                        filters,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields
                    });
            }
        }
    }
}