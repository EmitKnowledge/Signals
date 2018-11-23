using System.Linq;
using System.Security.Claims;

namespace Signals.Aspects.Auth.Mvc5
{
    /// <summary>
    /// Authorization manager
    /// </summary>
    public class AuthorizationManager : IAuthorizationManager
    {
        /// <summary>
        /// Authentication manager
        /// </summary>
        private readonly IAuthenticationManager _authenticationManager;

	    /// <summary>
	    /// CTOR
	    /// </summary>
	    /// <param name="authenticationManager"></param>
	    public AuthorizationManager(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        /// <summary>
        /// Add roles to currently logged in user
        /// </summary>
        /// <param name="roles"></param>
        public void AddRoles(params string[] roles)
        {
            if (roles == null) return;
            var principal = _authenticationManager.GetCurrentPrincipal();
            AddRoles(principal, roles);
        }

        /// <summary>
        /// Add roles to principal
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="roles"></param>
        public void AddRoles(ClaimsPrincipal principal, params string[] roles)
        {
            AddRoles(principal.Identity as ClaimsIdentity, roles);
        }

        /// <summary>
        /// Add roles to identity
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="roles"></param>
        public void AddRoles(ClaimsIdentity identity, params string[] roles)
        {
            foreach (var role in roles)
            {
                var claim = identity?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role && x.Value == role);
				if (claim == null)
				{
					identity?.AddClaim(new Claim(ClaimTypes.Role, role));
				}
            }
        }

        /// <summary>
        /// Check role for currently logged in user
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool HasRole(string role)
        {
            var principal = _authenticationManager.GetCurrentPrincipal();
            return principal.IsInRole(role);
        }

        /// <summary>
        /// Remove roles to currently logged in user
        /// </summary>
        /// <param name="roles"></param>
        public void RemoveRoles(params string[] roles)
        {
            if (roles == null) return;
            var principal = _authenticationManager.GetCurrentPrincipal();
            var identity = principal.Identity as ClaimsIdentity;

            foreach (var role in roles)
            {
                var claim = principal.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role && x.Value == role);
				if (claim != null)
				{
					identity?.RemoveClaim(claim);
				}
            }
        }
    }
}
