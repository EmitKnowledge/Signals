using Signals.Aspects.Auth;
using Signals.Aspects.DI;
using System;
using System.Linq;

namespace Signals.Core.Processing.Authorization
{
    /// <summary>
    /// Process authorizaiton attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SignalsAuthorizeAttribute : Attribute
    {
        private object[] Roles { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="roles"></param>
        public SignalsAuthorizeAttribute(params object[] roles)
        {
	        if (roles.Any(x => !(x is Enum)))
	        {
		        var notEnum = roles.FirstOrDefault(x => !(x is Enum));
		        this.D($"Type: {notEnum?.GetType()?.FullName} must be of type enum for the roles arguments.");
                throw new ArgumentException("Enum types must be provided for the roles arguments.");
	        }

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
            var hasPermission = permissionManager.HasPermission(feature, Roles.Cast<Enum>().ToArray());
            this.D($"Has permission for: {feature} -> {hasPermission}.");
            return hasPermission;
        }
    }
}