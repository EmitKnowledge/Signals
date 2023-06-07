using Signals.Aspects.Auth;
using Signals.Aspects.DI;
using System;

namespace Signals.Core.Processing.Authentication
{
    /// <summary>
    /// Process authentication attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SignalsAuthenticateAttribute : Attribute
    {
        /// <summary>
        /// Authentication callback
        /// </summary>
        /// <returns></returns>
        internal bool Authenticate()
        {
            var isAuthenticated = SystemBootstrapper.GetInstance<IAuthenticationManager>()?
													.GetCurrentPrincipal()?
													.Identity?
													.IsAuthenticated == true;

            this.D($"Is authenticated identity -> {isAuthenticated}.");

            return isAuthenticated;
        }
    }
}