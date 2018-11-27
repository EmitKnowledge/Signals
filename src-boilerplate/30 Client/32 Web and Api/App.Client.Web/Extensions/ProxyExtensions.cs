using System.Web.Mvc;

namespace App.Client.Web.Extensions
{
    public static class ProxyExtensions
    {
        /// <summary>
        /// Return an proxyified image url 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ProxyImage(this HtmlHelper helper, string url)
        {
            var encodedUrl = helper.ViewContext.RequestContext.HttpContext.Server.UrlEncode(url);
            
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            var proxifiedUrl = urlHelper.Action(@"Image", @"Proxy", new
            {
                url = encodedUrl,
                area = ""
            }, helper.ViewContext.RequestContext.HttpContext.Request.Url.Scheme);
            return proxifiedUrl;

        }
    }
}