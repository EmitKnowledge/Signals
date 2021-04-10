using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Signals.Aspects.Auth.NetCore.CustomScheme
{
    /// <summary>
    /// Authenticaiton handler wrapped that uses custom principal provider
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SignalsAuthenticationHandler<T> : AuthenticationHandler<AuthenticationSchemeOptions> where T : SignalsPrincipalProvider
    {
        /// <summary>
        /// User defined principal provider
        /// </summary>
        private readonly SignalsPrincipalProvider _customPrincipalProvider;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        /// <param name="customPrincipalProvider"></param>
        public SignalsAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, T customPrincipalProvider)
            : base(options, logger, encoder, clock)
        {
            _customPrincipalProvider = customPrincipalProvider;
        }

        /// <summary>
        /// Authentication request handler
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Context?.User?.Identity?.IsAuthenticated == true)
            {
                return await Task.FromResult(AuthenticateResult.NoResult());
            }
            
            var principal = _customPrincipalProvider.GetPrincipal(Context?.Request);

            if (principal == null)
            {
                return await Task.FromResult(AuthenticateResult.NoResult());
            }

            return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name)));
        }
    }
}
