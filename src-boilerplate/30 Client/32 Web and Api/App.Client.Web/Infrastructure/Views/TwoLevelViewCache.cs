using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace App.Client.Web.Infrastructure.Views
{
    /// <summary>
    /// Implement views caching
    /// </summary>
    public class TwoLevelViewCache : IViewLocationCache
    {
        private readonly static object SKey = new object();
        private readonly IViewLocationCache _cache;

        public TwoLevelViewCache(IViewLocationCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Return the context cache
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static IDictionary<string, string> GetRequestCache(HttpContextBase httpContext)
        {
            var d = httpContext.Items[SKey] as IDictionary<string, string>;
            if (d == null)
            {
                d = new Dictionary<string, string>();
                httpContext.Items[SKey] = d;
            }
            return d;
        }

        /// <summary>
        /// Return view location
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetViewLocation(HttpContextBase httpContext, string key)
        {
            var d = GetRequestCache(httpContext);
            string location;
            if (!d.TryGetValue(key, out location))
            {
                location = _cache.GetViewLocation(httpContext, key);
                d[key] = location;
            }
            return location;
        }

        /// <summary>
        /// Inserts the specified view location into the cache by using the specified HTTP context and the cache key.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param><param name="key">The cache key.</param><param name="virtualPath">The virtual path.</param>
        public void InsertViewLocation(HttpContextBase httpContext, string key, string virtualPath)
        {
            _cache.InsertViewLocation(httpContext, key, virtualPath);
        }
    }
}