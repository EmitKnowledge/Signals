using System.Web;
using System.Web.Routing;

namespace App.Client.Web.Infrastructure.Routing
{
    public class SubdomainRouteConstraint : IRouteConstraint
    {
        private string Subdomain { get; set; }

        public SubdomainRouteConstraint(string subdomain)
        {
            Subdomain = subdomain;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
                          RouteDirection routeDirection)
        {
            // get the requesting url
            string url = httpContext.Request.Headers["HOST"];
            int index = url.IndexOf(".", System.StringComparison.Ordinal);

            if (index < 0)
            {
                return false;
            }

            // no subdomain has been detected
            var args = url.Split('.');
            if (args.Length < 2) return false;

            string sub = args[0];
            // blacklisted subdomains kill the constraint matching
            if (sub == "local" || 
                sub == "dev" || 
                sub == "www")
            {
                return false;
            }

            if (sub != Subdomain) return false;

            // propagate the subdomain
            if (!values.ContainsKey("subdomainAsAParameter"))
            {
                values.Add("subdomainAsAParameter", sub);
            }

            return true;
        }
    }
}