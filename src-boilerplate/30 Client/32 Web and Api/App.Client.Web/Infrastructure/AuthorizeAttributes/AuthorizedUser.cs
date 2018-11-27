using System;
using System.Web;
using System.Web.Mvc;
using App.Client.SecurityProvider.Provider;

namespace App.Client.Web.Infrastructure.AuthorizeAttributes
{
    /// <summary>
    /// Controller action calls for registered user
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizedUser : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (MvcFormsSecurityProvider.CurrentUser == null) return false;
            return MvcFormsSecurityProvider.CurrentUser.IsAuthenticated &&
                  !MvcFormsSecurityProvider.CurrentUser.IsAnonymous;
        }
    }
}
