using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Signals.Aspects.Auth.Exceptions;
using Signals.Aspects.Auth.NetCore.CustomScheme;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Aspects.Auth.NetCore.Extensions
{
    /// <summary>
    /// Extension class for registering authentication schemes
    /// </summary>
    public static class RegistrationExtension
    {
        /// <summary>
        /// List of ordered authenticaiton schemes
        /// </summary>
        internal static List<string> RegisteredSchemes { get; set; } = new List<string>();

        /// <summary>
        /// Cookie authentication builder
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddSignalsAuth(this AuthenticationBuilder builder, Action<CookieAuthenticationOptions> options)
        {
            var success = AddRegisteredSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!success) throw new SchemeAlreadyRegisteredException(CookieAuthenticationDefaults.AuthenticationScheme);

            builder
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options);
            
            return builder;
        }

        /// <summary>
        /// Custom authentication builder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="schemeName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddSignalsAuth<T>(this AuthenticationBuilder builder, string schemeName, Action<AuthenticationSchemeOptions> options) where T : SignalsPrincipalProvider
        {
            var success = AddRegisteredSchemes(schemeName);
            if (!success) throw new SchemeAlreadyRegisteredException(schemeName);

            builder.Services.AddTransient<T>();

            builder
                .AddScheme<AuthenticationSchemeOptions, SignalsAuthenticationHandler<T>>(schemeName, options);

            return builder;
        }

        /// <summary>
        /// Registers authentication middleware
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSignalsAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();

            return app;
        }

        /// <summary>
        /// Adds scheme and reorders it so "Cookies" scheme is always last
        /// </summary>
        /// <param name="schemeName"></param>
        /// <returns></returns>
        private static bool AddRegisteredSchemes(string schemeName)
        {
            if (RegisteredSchemes.Contains(schemeName)) return false;

            RegisteredSchemes.Add(schemeName);
            RegisteredSchemes = RegisteredSchemes.Distinct().ToList();
            RegisteredSchemes.Sort();

            if (RegisteredSchemes.Contains(CookieAuthenticationDefaults.AuthenticationScheme))
            {
                RegisteredSchemes.Remove(CookieAuthenticationDefaults.AuthenticationScheme);
                RegisteredSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return true;
        }
    }
}
