using System.Linq;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http.Controllers;

namespace App.Client.Web.Infrastructure.AuthorizeAttributes
{
    /// <summary>
    /// Ajax based CSRF token
    /// </summary>
    public class ValidateAjaxAntiForgeryToken : System.Web.Http.AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
	        if (actionContext.Request.Method == HttpMethod.Get) return;
	        if (actionContext.Request.Method == HttpMethod.Head) return;
	        if (actionContext.Request.Method == HttpMethod.Options) return;
	        if (actionContext.Request.Method == HttpMethod.Trace) return;

			var headerToken = actionContext
                .Request
                .Headers
                .GetValues("__RequestVerificationToken")
                .FirstOrDefault();

            var cookieToken = actionContext
                .Request
                .Headers
                .GetCookies()
                .Select(c => c[AntiForgeryConfig.CookieName])
                .FirstOrDefault();

            // check for missing cookie or header
            if (cookieToken == null || headerToken == null)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }

            // ensure that the cookie matches the header
            try
            {
                AntiForgery.Validate(cookieToken.Value, headerToken);
            }
            catch
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}