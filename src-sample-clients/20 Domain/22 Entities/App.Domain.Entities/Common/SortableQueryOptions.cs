using System;
using System.Runtime.Serialization;

namespace App.Domain.Entities.Common
{
    [DataContract, Serializable]
    public class SortableQueryOptions : QueryOptions
    {
        /// <summary>
        /// Define the order column
        /// </summary>
        [DataMember]
        public string OrderBy { get; set; }

        /// <summary>
        /// Define the sort order
        /// </summary>
        [DataMember]
        public OrderBy Order { get; set; }
    }

    /// <summary>
    /// Data ordering
    /// </summary>
    [DataContract, Serializable]
    public enum OrderBy
    {
        /// <summary>
        /// No order by applied
        /// </summary>
        [EnumMember]
        None,

        /// <summary>
        /// Order by ascending
        /// </summary>
        [EnumMember]
        Asc,

        /// <summary>
        /// Order by descending
        /// </summary>
        [EnumMember]
        Desc
    }
}