using Signals.Aspects.Auth;
using Signals.Aspects.DI;
using System;
using System.Linq;

namespace Signals.Core.Processing.Authorization
{
    /// <summary>
    /// Process authorizaiton attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SignalsAuthorizeProcessAttribute : Attribute
    {
        private object[] Roles { get; }
        
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="roles"></param>
        public SignalsAuthorizeProcessAttribute(params object[] roles)
        {
            if (roles.Any(x => !(x is Enum)))
                throw new ArgumentException("Enum types must be provided for the roles arguments.");

            Roles = roles;
        }

        /// <summary>
        /// Authorization callback
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        internal bool Authorize(string feature)
        {
            var permissionManager = SystemBootstrapper.GetInstance<IPermissionManager>();
            return permissionManager.HasPermission(feature, Roles.Cast<Enum>().ToArray());
        }
    }
}