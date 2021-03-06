﻿using System.Collections.Generic;
using ExaltedCharm.Api.Services;

namespace ExaltedCharm.Api.PropertyMappings
{
    public static class AbilityMappings
    {
        public static Dictionary<string, PropertyMappingValue> CasteToCasteTypeDto()
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