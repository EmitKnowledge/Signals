using System.Security.Claims;
using System.Web;

namespace Signals.Aspects.Auth.Mvc5.CustomScheme
{
    /// <summary>
    /// Base for creating principal provider out of http request
    /// </summary>
    public abstract class SignalsPrincipalProvider
    {
        /// <summary>
        /// Principal provider handler
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public abstract ClaimsPrincipal GetPrincipal(HttpRequest httpRequest);
    }
}
