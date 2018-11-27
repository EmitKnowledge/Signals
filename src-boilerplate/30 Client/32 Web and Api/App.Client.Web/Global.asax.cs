using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using App.Client.SecurityProvider.Provider;
using App.Common.Base.Logging;

namespace App.Client.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // init the web service container (web app as service bootstrapped)
            AppWebService.Init();
            // register API routes
            AppWebService.RegisterApiRoutes();
            // register all defined areas
            AppWebService.RegisterAllAreas();
            // register system routes
            AppWebService.RegisterRoutes(RouteTable.Routes);
            // register global action filters
            AppWebService.RegisterGlobalFilters(GlobalFilters.Filters);
            // register all automap class mappings
            AppWebService.RegisterAutoMapperMaps();
            // register all custom model binders
            AppWebService.RegisterModelBinders();
            // register oauth providers
            AppWebService.RegisterOAuth();
            // register css/script bundles
            AppWebService.RegisterBundles();
            // strip mvc headers from response
            AppWebService.StripHeaders();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (exception != null)
            {
                var entry = LogEntry.Exception(ex: exception);
                entry.UserId = MvcFormsSecurityProvider.CurrentUser.Id;
                AppWebService.Instance.Logger.Error(entry);
            }
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            // check for ticket renewals and set the current principal
            MvcFormsSecurityProvider.SetCurrentPrincipalAndPrincipalTicket();
        }

        protected void Application_EndRequest()
        {
            var context = new HttpContextWrapper(this.Context);

            // If we're an ajax request and forms authentication caused a 302, 
            // then we actually need to do a 401
            if (FormsAuthentication.IsEnabled && context.Response.StatusCode == (int)HttpStatusCode.Found
                && context.Request.IsAjaxRequest())
            {
                context.Response.Clear();
                context.Response.StatusCode = 401;
            }
        }
    }
}
