using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Signals.Aspects.Auth.Mvc5.CustomScheme;

namespace Signals.Aspects.Auth.Mvc5.Extensions
{
    /// <summary>
    /// Extension class for registering authentication schemes
    /// </summary>
    public static class RegistrationExtension
    {
        /// <summary>
        /// Cookie authentication builder
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IAppBuilder AddSignalsAuth(this IAppBuilder builder, CookieAuthenticationOptions options)
        {
            builder.UseCookieAuthentication(options);
            return builder;
        }

        /// <summary>
        /// Custom authentication builder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IAppBuilder AddSignalsAuth<T>(this IAppBuilder builder, AuthenticationOptions options) where T : SignalsPrincipalProvider
        {
            builder.Use<SignalsMiddleware<T>>(options);
            return builder;
        }
    }
}
