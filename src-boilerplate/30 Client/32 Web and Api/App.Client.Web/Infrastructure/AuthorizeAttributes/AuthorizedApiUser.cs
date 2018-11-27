using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using App.Client.SecurityProvider.Provider;

namespace App.Client.Web.Infrastructure.AuthorizeAttributes
{
    /// <summary>
    /// Controller action calls for registered user
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizedApiUserAttribute : System.Web.Http.AuthorizeAttribute
    {
        /// <summary>
        /// Calls when an action is being authorized.
        /// </summary>
        /// <param name="actionContext">The context.</param><exception cref="T:System.ArgumentNullException">The context parameter is null.</exception>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            bool shouldFault = true;

            if (shouldFault)
            {
                if (!MvcFormsSecurityProvider.CurrentUser.IsAuthenticated ||
                     MvcFormsSecurityProvider.CurrentUser.IsAnonymous)
                {
                    shouldFault = true;
                }
                else
                {
                    shouldFault = false;
                }
            }

            if (shouldFault)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}
