using App.Common.Base.Repository;

namespace App.Client.Web.Infrastructure.Queries
{
    /// <summary>
    /// Preconfigured query options for data retrival
    /// </summary>
    public static class PreconfiguredQueryOptions
    {
        /// <summary>
        /// Return default query options configuration
        /// </summary>
        public static QueryOptions DefaultQueryOptions
        {
            get
            {
                var queryOptions = new QueryOptions();
                queryOptions.Page = 1;
                queryOptions.PageSize = 10;
                return queryOptions;
            }
        }
    }
}