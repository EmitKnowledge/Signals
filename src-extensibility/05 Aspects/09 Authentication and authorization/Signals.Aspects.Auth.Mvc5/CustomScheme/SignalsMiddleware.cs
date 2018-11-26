using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace Signals.Aspects.Auth.Mvc5.CustomScheme
{
    /// <summary>
    /// OWIN middleware wrapper for custom principal provider
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SignalsMiddleware<T> : AuthenticationMiddleware<AuthenticationOptions> where T : SignalsPrincipalProvider
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="nextMiddleware"></param>
        /// <param name="authOptions"></param>
        public SignalsMiddleware(OwinMiddleware nextMiddleware, AuthenticationOptions authOptions)
            : base(nextMiddleware, authOptions)
        {

        }

		/// <summary>
		/// Creates instance of <see cref="SignalsAuthenticationHandler{T}"/> using user defined principal provider
		/// </summary>
		/// <returns></returns>
		protected override AuthenticationHandler<AuthenticationOptions> CreateHandler()
        {
            return new SignalsAuthenticationHandler<T>();
        }
    }
}
