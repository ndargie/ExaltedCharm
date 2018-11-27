﻿using System;
using System.Collections.Generic;
using System.Linq;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Models;

namespace ExaltedCharm.Api.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<Duration, DurationDto>(_durationPropertyMapping));
        }

        private readonly Dictionary<string, PropertyMappingValue> _durationPropertyMapping = new Dictionary<string, PropertyMappingValue>()
        {
            { "Id", new PropertyMappingValue(new List<string>() { "Id" }) },
            { "Name", new PropertyMappingValue(new List<string>() { "Name" }) },
            { "Description", new PropertyMappingValue(new List<string>() { "Description" }) },
        };

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>().ToList();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }

            throw new Exception($"Cannot find exact proerty mapping for <{typeof(TSource)}>");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            bool result = true;
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit)
                {
                    var trimmedField = field.Trim();

                    var indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal);
                    var propertyName = indexOfFirstSpace == -1 ?
                        trimmedField : trimmedField.Remove(indexOfFirstSpace);

                    if (!propertyMapping.ContainsKey(propertyName))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }
    }
}