using System.Collections.Generic;
using ExaltedCharm.Api.Services;

namespace ExaltedCharm.Api.PropertyMappings
{
    public static class RangedWeaponMappings
    {
        public static Dictionary<string, PropertyMappingValue> RangedWeaponToRangedWeaponDto()
        {
            return new Dictionary<string, PropertyMappingValue>()
            {
                {"Id", new PropertyMappingValue(new List<string>( ) {"Id"})},
                {"Name", new PropertyMappingValue(new List<string>() {"Name"})},
                {"Description", new PropertyMappingValue(new List<string>() {"Description"})},
                {"Rate", new PropertyMappingValue(new List<string>() {"Rate"})},
                {"Accuracy", new PropertyMappingValue(new List<string>() {"Accuracy"})},
                {"Damage", new PropertyMappingValue(new List<string>() {"Damage"})},
                {"DamageType", new PropertyMappingValue(new List<string>() {"DamageType"})},
                {"Range", new PropertyMappingValue(new List<string>() {"Range"})}
            };
        }
    }
}