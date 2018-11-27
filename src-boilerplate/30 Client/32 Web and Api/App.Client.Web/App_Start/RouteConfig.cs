using System.Web.Mvc;
using System.Web.Routing;

namespace App.Client.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.LowercaseUrls = true;
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*sitemap}", new { sitemap = @"(.*/)?sitemap.xml(/.*)?" });

            routes.MapRoute(
                "login",
                "login",
                new { controller = "Account", action = "Login" }
            );

            routes.MapRoute(
                "logout",
                "logout",
                new { controller = "Account", action = "Logout" }
            );            

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
