using System;
using System.IO;
using System.IO.Compression;
using System.Web;

namespace App.Client.Web.Infrastructure.HttpModules
{
    /// <summary>
    /// GZIP compression for svg 
    /// <add name="SvgCompressionModule" type="App.Client.Web.Infrastructure.HttpModules.SvgCompressionModule, App.Client.Web" />
    /// </summary>
    public class SvgCompressionModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.BeginRequest += ApplicationOnBeginRequest;
        }

        public void Dispose()
        {

        }
        private void ApplicationOnBeginRequest(object sender, EventArgs eventArgs)
        {
            var app = (HttpApplication)sender;
            if (app == null) return;
            var context = app.Context;
            if (!IsSvgRequest(context.Request)) return;
            context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
            context.Response.AddHeader("Content-encoding", "gzip");
        }

        /// <summary>
        /// Check if the request is an svg request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual bool IsSvgRequest(HttpRequest request)
        {
            var path = request.Url.AbsolutePath;
            return Path.HasExtension(path) &&
            Path.GetExtension(path).Equals(".svg", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
