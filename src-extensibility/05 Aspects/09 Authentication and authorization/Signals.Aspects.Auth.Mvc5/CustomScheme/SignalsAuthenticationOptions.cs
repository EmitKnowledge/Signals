using Microsoft.Owin.Security;

namespace Signals.Aspects.Auth.Mvc5.CustomScheme
{
    /// <summary>
    /// Implementation of authentication options
    /// </summary>
    public class SignalsAuthenticationOptions : AuthenticationOptions
    {
        /// <summary>
        /// Default authentication properties
        /// </summary>
        public AuthenticationProperties DefaultProperties { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="authenticationType"></param>
        public SignalsAuthenticationOptions(string authenticationType) : base(authenticationType)
        {

        }
    }
}
