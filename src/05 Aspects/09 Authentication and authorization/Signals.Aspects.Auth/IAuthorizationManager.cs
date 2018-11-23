using System.Security.Claims;

namespace Signals.Aspects.Auth
{
    /// <summary>
    /// Authorization manager
    /// </summary>
    public interface IAuthorizationManager
    {
        /// <summary>
        /// Add roles to currently logged in user
        /// </summary>
        /// <param name="roles"></param>
        void AddRoles(params string[] roles);

        /// <summary>
        /// Add roles to principal
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="roles"></param>
        void AddRoles(ClaimsPrincipal principal, params string[] roles);

        /// <summary>
        /// Add roles to identity
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="roles"></param>
        void AddRoles(ClaimsIdentity identity, params string[] roles);

        /// <summary>
        /// Check role for currently logged in user
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        bool HasRole(string role);

        /// <summary>
        /// Remove roles to currently logged in user
        /// </summary>
        /// <param name="roles"></param>
        void RemoveRoles(params string[] roles);
    }
}
