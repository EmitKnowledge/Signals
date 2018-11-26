using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Signals.Aspects.Auth.Mvc5.CustomScheme
{
    /// <summary>
    /// Authenticaiton handler wrapped that uses custom principal provider
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SignalsAuthenticationHandler<T> : AuthenticationHandler<AuthenticationOptions> where T : SignalsPrincipalProvider
    {
        /// <summary>
        /// User defined principal provider
        /// </summary>
        private readonly SignalsPrincipalProvider _customPrincipalProvider;

        /// <summary>
        /// CTOR
        /// </summary>
        public SignalsAuthenticationHandler()
        {
            _customPrincipalProvider = DependencyResolver.Current.GetService<T>();
        }

        /// <summary>
        /// Authentication request pre-handler
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> InvokeAsync()
        {
            var ticket = await AuthenticateAsync();

	        if (ticket == null) return false;
	        Context.Authentication.SignIn(ticket.Properties, ticket.Identity);

	        // Prevent further processing by the owin pipeline.
			if (string.IsNullOrEmpty(ticket.Properties.RedirectUri)) return false;
	        Response.Redirect(ticket.Properties.RedirectUri);
			
			// Let the rest of the pipeline run.
			return true;
        }

        /// <summary>
        /// Authentication request handler
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            // If user is already authenticated, do nothing
            if (Context?.Request?.User?.Identity?.IsAuthenticated == true)
            {
                return await Task.FromResult<AuthenticationTicket>(null);
            }

            // if user defined principal provider doesn't return principal, do nothing
            var principal = _customPrincipalProvider.GetPrincipal(HttpContext.Current.Request);

            if (principal == null)
            {
                return await Task.FromResult<AuthenticationTicket>(null);
            }

            // else authenticate principal
            var options = Options as SignalsAuthenticationOptions;
            return await Task.FromResult(new AuthenticationTicket(principal.Identity as ClaimsIdentity, options?.DefaultProperties));
        }
    }
}
