﻿using System.Collections.Generic;
using ExaltedCharm.Api.Services;

namespace ExaltedCharm.Api.PropertyMappings
{
    public static class ExaltedTypeMappings
    {
        public static Dictionary<string, PropertyMappingValue> ExaltedTypeToExaltedTypeDto()
        {
            return new Dictionary<string, PropertyMappingValue>()
            {
                {"Id", new PropertyMappingValue(new List<string>( ) {"Id"})},
                {"Name", new PropertyMappingValue(new List<string>() {"Name"})},
                {"Description", new PropertyMappingValue(new List<string>() {"Description"})}
            };
        }
    }
}