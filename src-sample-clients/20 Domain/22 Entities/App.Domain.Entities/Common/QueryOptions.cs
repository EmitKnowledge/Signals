using System;
using System.Runtime.Serialization;

namespace App.Domain.Entities.Common
{
    /// <summary>
    /// Search query options
    /// </summary>
    [Serializable]
    [DataContract]
    public class QueryOptions
    {
        /// <summary>
        /// Represent the query string of the search request
        /// </summary>
        [DataMember]
        public string Query { get; set; }

        /// <summary>
        /// Page to be retrieved
        /// </summary>
        [DataMember]
        public int Page { get; set; }

        /// <summary>
        /// Pagesize - result count
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public QueryOptions()
        {
            Page = 1;
            PageSize = 10;
        }

        /// <summary>
        /// Check if pagination values are valid and override with provided values if not
        /// </summary>
        public void OverridePagingValuesIfNotValid(int page = 1, int pageSize = 10)
        {
            if (Page < 1)
            {
                Page = page;
            }
            if (PageSize < 1)
            {
                PageSize = pageSize;
            }
        }
    }
}