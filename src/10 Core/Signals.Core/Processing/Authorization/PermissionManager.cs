using Signals.Aspects.Auth;
using Signals.Aspects.Auth.Extensions;
using Signals.Aspects.Security;
using System;
using System.Linq;

namespace Signals.Core.Processing.Authorization
{
    /// <summary>
    /// Represents permission wrapper
    /// </summary>
    public class PermissionManager : IPermissionManager
    {
        /// <summary>
        /// Provider that provides permission operations
        /// </summary>
        private readonly IPermissionProvider _permissionProvider;

        /// <summary>
        /// Authentication manager
        /// </summary>
        private readonly IAuthenticationManager _authenticationManager;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="permissionProvider"></param>
        /// <param name="authenticationManager"></param>
        public PermissionManager(IPermissionProvider permissionProvider, IAuthenticationManager authenticationManager)
        {
            _permissionProvider = permissionProvider;
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
            _permissionProvider.SetUserPermission(userIdentifier, feature, hasAccess);
        }

        /// <summary>
        /// Creates of updates permission in the system
        /// </summary>
        /// <param name="role"></param>
        /// <param name="feature"></param>
        /// <param name="hasAccess"></param>
        public void SetRolePermission(string role, string feature, bool hasAccess)
        {
            _permissionProvider.SetRolePermission(role, feature, hasAccess);
        }

        /// <summary>
        /// Removes user permission from the system
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="feature"></param>
        public void RemoveUserPermission(string userName, string feature)
        {
            _permissionProvider.RemoveUserPermission(userName, feature);
        }

        /// <summary>
        /// Removes role permission from the system
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="feature"></param>
        public void RemoveRolePermission(string roleName, string feature)
        {
            _permissionProvider.RemoveRolePermission(roleName, feature);
        }

        /// <summary>
        /// Checks if the user has permission for the feature
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public bool HasPermission(string feature, Enum[] roles = null)
        {
            var currentPrincipal = _authenticationManager.GetCurrentPrincipal();
            if (currentPrincipal == null || currentPrincipal.Identity?.IsAuthenticated == false) return false;

            return roles?.Select(x => x.ToString()).Any(currentPrincipal.IsInRole)
                   ?? _permissionProvider.HasPermission(currentPrincipal.Identity?.Name, feature, currentPrincipal.GetRoles());
        }
    }
}