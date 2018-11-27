using System.Web.Mvc;
using System.Web.Routing;

namespace App.Client.Web.Infrastructure.Routing
{
    /// <summary>
    /// Route extension methods
    /// </summary>
    public static class RouteExtension
    {
        /// <summary>
        /// Map a route to lowercase
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <returns></returns>
        public static void MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults)
        {
            routes.MapRouteLowerCase(name, url, defaults, null);
        }

        /// <summary>
        /// Map a route to lowercase and language
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <returns></returns>
        public static void MapRouteLowerCaseLocalized(this RouteCollection routes, string name, string url, object defaults)
        {
            var localizedName = string.Concat(@"lang-", name);
            var localizedUrl = string.Concat(@"{lang}/", url);
            var localizationConstraints = new { lang = @"(\w{2})|(\w{2}-\w{2})" };

            // register with language
            routes.MapRouteLowerCase(localizedName, localizedUrl, defaults, localizationConstraints);

            // register base without language
            routes.MapRouteLowerCase(name, url, defaults, null);
        }

        /// <summary>
        /// Map a route to lowercase
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="constraints"></param>
        /// <returns></returns>
        public static void MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults, object constraints)
        {
            Route route = new LowercaseRoute(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints)
            };

            routes.Add(name, route);
        }

        /// <summary>
        /// Map a route to lowercase and language
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <returns></returns>
        public static void MapRouteLocalized(this RouteCollection routes, string name, string url, object defaults)
        {
            var localizedName = string.Concat(@"lang-", name);
            var localizedUrl = string.Concat(@"{lang}/", url);
            var localizationConstraints = new { lang = @"(\w{2})|(\w{2}-\w{2})" };

            // register with language
            routes.MapRoute(localizedName, localizedUrl, defaults, localizationConstraints);

            // register base without language
            routes.MapRoute(name, url, defaults, null);
        }
    }
}