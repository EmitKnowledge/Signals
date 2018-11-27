using System;
using System.Web.Mvc;

namespace App.Client.Web.Infrastructure.Filters
{
    public class RequireHttpsAttributeEx : RequireHttpsAttribute
    {
        private const int HttpsDefaultPort = 443;

        private int HttpsPort { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public RequireHttpsAttributeEx()
        {
            HttpsPort = HttpsDefaultPort;
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="httpsPort"></param>
        public RequireHttpsAttributeEx(int httpsPort)
        {
            HttpsPort = httpsPort;
        }

        protected override void HandleNonHttpsRequest(AuthorizationContext filterContext)
        {
            // The base only redirects GET, but we added HEAD as well. This avoids exceptions for bots crawling using HEAD.
            // The other requests will throw an exception to ensure the correct verbs are used. 
            // We fall back to the base method as the mvc exceptions are marked as internal. 

            if (!string.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) && 
                !string.Equals(filterContext.HttpContext.Request.HttpMethod, "HEAD", StringComparison.OrdinalIgnoreCase))
            {
                base.HandleNonHttpsRequest(filterContext);
            }

            string url;
            if (HttpsPort == HttpsDefaultPort)
            {
                url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
            }
            else
            {
                url = "https://" + filterContext.HttpContext.Request.Url.Host + $":{HttpsPort}" + filterContext.HttpContext.Request.RawUrl;
            }
            

            // Redirect to HTTPS version of page
            // We updated this to redirect using 301 (permanent) instead of 302 (temporary).
            filterContext.Result = new RedirectResult(url, true);
        }
    }
}