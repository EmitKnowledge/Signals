using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace App.Domain.Entities.Common
{
    /// <summary>
    /// Represent a search request class
    /// </summary>
    [DataContract, Serializable]
    public class AdvancedQueryOptions : SortableQueryOptions
    {
        /// <summary>
        /// Represents the column filters
        /// </summary>
        [DataMember]
        public List<SearchRequestFilter> Filters { get; set; }

        /// <summary>
        /// When no filters are specified we search on multiple fields predefined by the repo query
        /// </summary>
        public bool IsIndexSearch => Filters.Count == 0;

        /// <summary>
        /// Check if a filter is defined
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool HasFilter(string filter)
        {
            return Filters.Any(x => x.Name == filter);
        }

        /// <summary>
        /// Return filter value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public T GetFilterValue<T>(string filter)
        {
            var existingFilter = Filters.FirstOrDefault(x => x.Name == filter);
            if (string.IsNullOrEmpty(existingFilter?.Value)) return default(T);
            Type t = typeof(T);
            // Get the type that was made nullable.
            Type valueType = Nullable.GetUnderlyingType(typeof(T));
            if (valueType != null)
            {
                // Convert to the value type.
                object result = Convert.ChangeType(existingFilter.Value, valueType);
                // Cast the value type to the nullable type.
                return (T)result;
            }
            // Not nullable.
            return (T)Convert.ChangeType(existingFilter.Value, typeof(T));
        }

        /// <summary>
        /// CTOR
        /// </summary>
        public AdvancedQueryOptions()
        {
            Filters = new List<SearchRequestFilter>();
        }
    }

    /// <summary>
    /// Advanced search request filtering
    /// </summary>
    [DataContract, Serializable]
    public class SearchRequestFilter
    {
        /// <summary>
        /// Name of the filter
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Filter value
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }

    /// <summary>
    /// Comparative type for filters. Exp: GreaterThan 1000
    /// </summary>
    [DataContract, Serializable]
    public enum FilterComparatorType
    {
        [EnumMember(Value = @">")]
        GreaterThan,

        [EnumMember(Value = @"<")]
        LessThan,

        [EnumMember(Value = @"=")]
        Is,

        [EnumMember(Value = @"!=")]
        IsNot
    }

    /// <summary>
    /// Represent the filter value type
    /// </summary>
    [DataContract, Serializable]
    public enum FilterType
    {
        [EnumMember]
        Number,

        [EnumMember]
        Select,

        [EnumMember]
        Text,

        [EnumMember]
        Currency
    }
}