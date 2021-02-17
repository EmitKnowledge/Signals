using App.Domain.DataRepositoryContracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Signals.Aspects.Auth;
using Signals.Aspects.DI;
using Signals.Core.Common.Instance;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace App.Client.Web.Infrastructure
{
    /// <summary>
    /// Token authentication sheme options
    /// </summary>
    public class TokenAuthOptions : AuthenticationSchemeOptions
    {
    }

    /// <summary>
    /// Token authentication scheme
    /// </summary>
    public class TokenAuthScheme : AuthenticationHandler<TokenAuthOptions>
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        public TokenAuthScheme(IOptionsMonitor<TokenAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = Request.Headers["auth-token"].FirstOrDefault();

            if (token.IsNullOrEmpty()) return AuthenticateResult.NoResult();

            var authenticationManager = SystemBootstrapper.GetInstance<IAuthenticationManager>();
            var authorizationManager = SystemBootstrapper.GetInstance<IAuthorizationManager>();
            var userRepository = SystemBootstrapper.GetInstance<IUserRepository>();

            var user = userRepository.GetByToken(token);

            if (user.IsNull())
                return AuthenticateResult.NoResult();

            if (user.LastAccessDate.HasValue)
            {
                if (user.RememberMe && user.LastAccessDate.Value.AddYears(1) < DateTime.UtcNow)
                {
                    //userRepository.UpdateUserToken(user.Id, string.Empty);
                    return AuthenticateResult.NoResult();
                }
                else if (!user.RememberMe && user.LastAccessDate.Value.AddDays(1) < DateTime.UtcNow)
                {
                    //userRepository.UpdateUserToken(user.Id, string.Empty);
                    return AuthenticateResult.NoResult();
                }
            }

            userRepository.UpdateLastAccessDate(token);

            if (!user.IsNull())
            {
                var identity = new ClaimsIdentity(Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                authenticationManager.Login(principal, user);
                authorizationManager.AddRoles(user.Type.ToString());

                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.NoResult();
        }

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}