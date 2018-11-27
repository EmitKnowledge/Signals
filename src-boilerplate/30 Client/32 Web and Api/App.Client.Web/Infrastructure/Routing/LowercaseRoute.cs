using System.Web.Routing;

namespace App.Client.Web.Infrastructure.Routing
{
    /// <summary>
    /// Represent a lowercase route implementation
    /// </summary>
    public class LowercaseRoute : Route
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="url"></param>
        /// <param name="routeHandler"></param>
        public LowercaseRoute(string url, IRouteHandler routeHandler)
            : base(url, routeHandler) { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="routeHandler"></param>
        public LowercaseRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler) { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="constraints"></param>
        /// <param name="routeHandler"></param>
        public LowercaseRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler) { }
        
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="constraints"></param>
        /// <param name="dataTokens"></param>
        /// <param name="routeHandler"></param>
        public LowercaseRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler) : base(url, defaults, constraints, dataTokens, routeHandler) { }

        /// <summary>
        /// Return virtual path for a route
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            VirtualPathData path = base.GetVirtualPath(requestContext, values);

            if (path != null)
                path.VirtualPath = path.VirtualPath.ToLowerInvariant();

            return path;
        }
    }
}