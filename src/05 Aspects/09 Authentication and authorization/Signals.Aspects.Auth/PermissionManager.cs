using System;
using System.Linq;

namespace Signals.Aspects.Auth
{
    /// <summary>
    /// Principal permission manager
    /// </summary>
    public class PermissionManager : IPermissionManager
    {
        /// <summary>
        /// Authentication manager
        /// </summary>
        private readonly IAuthenticationManager _authenticationManager;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="authenticationManager"></param>
        public PermissionManager(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        /// <summary>
        /// Creates of updates permission in the system
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <param name="feature"></param>
        /// <param name="hasAccess"></param>
        public void SetUserPermission(string userIdentifier, string feature, bool hasAccess)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates of updates permission in the system
        /// </summary>
        /// <param name="role"></param>
        /// <param name="feature"></param>
        /// <param name="hasAccess"></param>
        public void SetRolePermission(string role, string feature, bool hasAccess)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes user permission from the system
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="feature"></param>
        public void RemoveUserPermission(string userName, string feature)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes role permission from the system
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="feature"></param>
        public void RemoveRolePermission(string roleName, string feature)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Checks if the user has permission for the feature
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public bool HasPermission(string feature, Enum[] roles)
        {
            var currentPrincipal = _authenticationManager.GetCurrentPrincipal();
            if (currentPrincipal == null || 
                currentPrincipal.Identity?.IsAuthenticated == false) return false;

			if (roles == null) return false;
	        if (roles.Length == 0) return false;

			return roles.Select(x => x.ToString()).Any(currentPrincipal.IsInRole);
        }
    }
}
