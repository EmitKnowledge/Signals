using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using App.Client.SecurityProvider.Provider;
using App.Service.DomainEntities.Users;

namespace App.Client.Web.Infrastructure.AuthorizeAttributes
{
    /// <summary>
    /// Controller action calls for registered user
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizedToAdministerApiUserAttribute : System.Web.Http.AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (MvcFormsSecurityProvider.CurrentUser == null)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                return;
            }
            if (!MvcFormsSecurityProvider.CurrentUser.IsAuthenticated || MvcFormsSecurityProvider.CurrentUser.IsAnonymous)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                return;
            }
            if (MvcFormsSecurityProvider.CurrentUser.Type != UserType.SystemAdministrator)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                return;
            }
        }
    }
}
