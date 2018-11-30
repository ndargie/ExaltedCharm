using System.Collections.Generic;
using ExaltedCharm.Api.Services;

namespace ExaltedCharm.Api.PropertyMappings
{
    public static class CharmMapping
    {
        public static Dictionary<string, PropertyMappingValue> CharmToCharmDto()
        {
            return new Dictionary<string, PropertyMappingValue>()
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"Name", new PropertyMappingValue(new List<string>() {"Name"})},
                {"Description", new PropertyMappingValue(new List<string>() {"Description"})},
                {"CharmTypeId", new PropertyMappingValue(new List<string>() {"CharmTypeId"})},
                {"Cost", new PropertyMappingValue(new List<string>() {"MoteCost", "WillpowerCost", "HealthCost"})}
            };
        }
    }
}