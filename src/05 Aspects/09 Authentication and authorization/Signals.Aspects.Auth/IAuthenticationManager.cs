﻿using System.Security.Claims;

namespace Signals.Aspects.Auth
{
    /// <summary>
    /// Authentication manager
    /// </summary>
    public interface IAuthenticationManager
    {
        /// <summary>
        /// Get currently logged in user principal
        /// </summary>
        /// <returns></returns>
        ClaimsPrincipal GetCurrentPrincipal();

        /// <summary>
        /// Get currently logged in user
        /// </summary>
        /// <returns></returns>
        T GetCurrentUser<T>() where T : class;

        /// <summary>
        /// Set currently logged in user
        /// </summary>
        /// <returns></returns>
        void SetCurrentUser<T>(T user) where T : class;

        /// <summary>
        /// Login user with addiitonal data
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="properties"></param>
        void Login(ClaimsPrincipal principal, AuthenticationProperties properties = null);

        /// <summary>
        /// Login user with addiitonal data
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="user"></param>
        /// <param name="properties"></param>
        void Login<T>(ClaimsPrincipal principal, T user, AuthenticationProperties properties = null) where T : class;

        /// <summary>
        /// Logout currently logged in user
        /// </summary>
        void Logout();
    }
}