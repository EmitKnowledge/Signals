using Signals.Aspects.Auth.Extensions;
using Signals.Aspects.Auth.Mvc5.Extensions;
using System;
using System.Security.Claims;
using System.Threading;
using System.Web;
using AuthenticationProperties = Signals.Aspects.Auth.AuthenticationProperties;

namespace Signals.Aspects.Auth.Mvc5
{
    /// <summary>
    /// Authentication manager
    /// </summary>
    public class AuthenticationManager : IAuthenticationManager
    {
        /// <summary>
        /// Get currently logged in user principal
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal GetCurrentPrincipal()
        {
            if (HttpContext.Current.User?.Identity?.IsAuthenticated == true) return HttpContext.Current.User as ClaimsPrincipal;
            if (Thread.CurrentPrincipal?.Identity?.IsAuthenticated == true) return Thread.CurrentPrincipal as ClaimsPrincipal;

            return null;
        }

        /// <summary>
        /// Get currently logged in user
        /// </summary>
        /// <returns></returns>
        public T GetCurrentUser<T>() where T : class
        {
            var principal = GetCurrentPrincipal();
            return principal?.GetClaim<T>(ClaimTypes.UserData);
        }

        /// <summary>
        /// Login user with addiitonal data
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="properties"></param>
        public void Login(ClaimsPrincipal principal, AuthenticationProperties properties = null)
        {
            Login<object>(principal, null, properties);
        }

        /// <summary>
        /// Login user with addiitonal data
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="user"></param>
        /// <param name="properties"></param>
        public void Login<T>(ClaimsPrincipal principal, T user, AuthenticationProperties properties = null) where T : class
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            if (user != null)
            {
                // add claims for user extracted from T user model @user
                (principal.Identity as ClaimsIdentity)?.AddClaims(user.ExtractClaims());
                principal.AddClaim(ClaimTypes.UserData, user);
            }
            if (properties != null) principal.AddClaim(PrincipalExtensions.AuthenticationPropertiesClaimName, properties);

            var claimProperties = principal.GetClaim<AuthenticationProperties>(PrincipalExtensions.AuthenticationPropertiesClaimName);

            if (HttpContext.Current != null)
            {
                HttpContext.Current.GetOwinContext().Authentication.SignIn(claimProperties.To(), principal.Identity as ClaimsIdentity);
                HttpContext.Current.User = principal;
            }

            Thread.CurrentPrincipal = principal;
        }

        /// <summary>
        /// Logout currently logged in user
        /// </summary>
        public void Logout()
        {
            HttpContext.Current.GetOwinContext().Authentication.SignOut(HttpContext.Current?.User?.Identity?.AuthenticationType);
            Thread.CurrentPrincipal = null;
            if (HttpContext.Current != null) HttpContext.Current.User = null;
        }

        /// <summary>
        /// Set currently logged user data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user"></param>
        public void SetCurrentUser<T>(T user) where T : class
        {
            var principal = GetCurrentPrincipal();
            if (principal == null) return;

            var claimProperties = principal.GetClaim<AuthenticationProperties>(PrincipalExtensions.AuthenticationPropertiesClaimName);
            Logout();
            Login(principal, user, claimProperties);
        }
    }
}
