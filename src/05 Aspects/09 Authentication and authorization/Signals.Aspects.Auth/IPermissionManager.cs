using System;

namespace Signals.Aspects.Auth
{
    public interface IPermissionManager
    {
        /// <summary>
        /// Creates of updates permission in the system
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <param name="feature"></param>
        /// <param name="hasAccess"></param>
        void SetUserPermission(string userIdentifier, string feature, bool hasAccess);

        /// <summary>
        /// Creates of updates permission in the system
        /// </summary>
        /// <param name="role"></param>
        /// <param name="feature"></param>
        /// <param name="hasAccess"></param>
        void SetRolePermission(string role, string feature, bool hasAccess);

        /// <summary>
        /// Removes user permission from the system
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="feature"></param>
        void RemoveUserPermission(string userName, string feature);

        /// <summary>
        /// Removes role permission from the system
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="feature"></param>
        void RemoveRolePermission(string roleName, string feature);

        /// <summary>
        /// Checks if the user has permission for the feature
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        bool HasPermission(string feature, Enum[] roles = null);
    }
}