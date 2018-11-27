using System;
using System.Web;
using System.Web.Mvc;

namespace App.Client.Web.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class CacheControlAttribute : ActionFilterAttribute
    {
        private HttpCacheability _cacheability;

        private DateTime? _expiresAt;

        public CacheControlAttribute(HttpCacheability cacheability)
        {
            _cacheability = cacheability;
        }

        public CacheControlAttribute(HttpCacheability cacheability, ExpiresAt expiresAtType)
        {
            _cacheability = cacheability;
            if (expiresAtType == ExpiresAt.EndOfDay)
            {
                _expiresAt = DateTime.Now.Date.AddHours(23);
            }
            else if (expiresAtType == ExpiresAt.TomorrowAndOfDay)
            {
                _expiresAt = DateTime.Now.Date.AddDays(1).AddHours(23);
            }
            else if (expiresAtType == ExpiresAt.OneYearFromToday)
            {
                _expiresAt = DateTime.Now.Date.AddYears(1).AddHours(23);
            }
        }

        public HttpCacheability Cacheability => this._cacheability;

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
            cache.SetCacheability(_cacheability);
            if(_expiresAt != null) cache.SetExpires(_expiresAt.Value);
        }
    }

    public enum ExpiresAt
    {
        EndOfDay,
        TomorrowAndOfDay,
        OneYearFromToday
    }
}
