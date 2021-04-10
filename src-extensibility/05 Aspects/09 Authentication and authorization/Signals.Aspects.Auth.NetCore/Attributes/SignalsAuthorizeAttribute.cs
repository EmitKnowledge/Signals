using Microsoft.AspNetCore.Authorization;
using Signals.Aspects.Auth.NetCore.Extensions;
using System;

namespace Signals.Aspects.Auth.NetCore.Attributes
{
    /// <summary>
    /// Authorize attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SignalsAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// The name of the operation/feature/process the user is accessing
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public SignalsAuthorizeAttribute()
        {
            Init();
        }

        /// <summary>
        /// CTOR
        /// </summary>
        public SignalsAuthorizeAttribute(string policy) : base(policy)
        {
            Init();
        }
        
        /// <summary>
        /// Initializes scheme invocaiton order
        /// </summary>
        public void Init()
        {
            if (string.IsNullOrEmpty(AuthenticationSchemes))
            {
                AuthenticationSchemes = string.Join(",", RegistrationExtension.RegisteredSchemes);
            }
        }
    }
}
