using System;
using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace App.Client.Web.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class CacheControlApiAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private readonly TimeSpan? _maxAge;

        public CacheControlApiAttribute(ExpiresAt expiresAtType)
        {
            _maxAge = TimeSpan.FromHours(1);

            var now = DateTime.Now.Date;
            if (expiresAtType == ExpiresAt.EndOfDay)
            {
                _maxAge = now.AddHours(23) - now;
            }
            else if (expiresAtType == ExpiresAt.TomorrowAndOfDay)
            {
                _maxAge = now.AddDays(1).AddHours(23) - now;
            }
            else if (expiresAtType == ExpiresAt.OneYearFromToday)
            {
                _maxAge = now.AddYears(1).AddHours(23) - now;
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Response != null)
                context.Response.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = _maxAge
                };

            base.OnActionExecuted(context);
        }
    }
}
