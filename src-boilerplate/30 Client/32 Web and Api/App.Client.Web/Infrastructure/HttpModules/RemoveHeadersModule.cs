using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Client.Web.Infrastructure.HttpModules
{
    public class RemoveHeadersModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.PostReleaseRequestState += new EventHandler(application_PostReleaseRequestState);

        }

        public void Dispose()
        {
        }

        void application_PostReleaseRequestState(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Remove("Server");
            HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
            HttpContext.Current.Response.Headers.Remove("ETag");
        }
    }
}