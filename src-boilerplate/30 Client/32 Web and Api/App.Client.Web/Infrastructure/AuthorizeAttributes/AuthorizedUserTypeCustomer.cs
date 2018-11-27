using System;
using System.Web;
using System.Web.Mvc;
using App.Client.SecurityProvider.Provider;
using App.Service.DomainEntities.Users;

namespace App.Client.Web.Infrastructure.AuthorizeAttributes
{
    /// <summary>
    /// Controller action calls for registered user
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizedUserTypeCustomer : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (MvcFormsSecurityProvider.CurrentUser == null) return false;
            return MvcFormsSecurityProvider.CurrentUser.IsAuthenticated &&
                  !MvcFormsSecurityProvider.CurrentUser.IsAnonymous &&
                  (MvcFormsSecurityProvider.CurrentUser.Type == UserType.User || 
                   MvcFormsSecurityProvider.CurrentUser.Type == UserType.SystemAdministrator);
        }
    }
}
