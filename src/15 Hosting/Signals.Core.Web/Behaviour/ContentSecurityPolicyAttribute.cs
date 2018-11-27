using Signals.Core.Common.Instance;
using System;

namespace Signals.Core.Web.Behaviour
{
    /// <summary>
    /// CSP header
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ContentSecurityPolicyAttribute : ResponseHeaderAttribute
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public ContentSecurityPolicyAttribute() : base("Content-Security-Policy", GenerateValue(null))
        {

        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="allowedSites"></param>
        public ContentSecurityPolicyAttribute(params string[] allowedSites) : base("Content-Security-Policy", GenerateValue(allowedSites))
        {
        }

        /// <summary>
        /// Generates string for allowed sites
        /// </summary>
        /// <param name="allowedSites"></param>
        /// <returns></returns>
        private static string GenerateValue(string[] allowedSites)
        {
            if (allowedSites.IsNullOrHasZeroElements()) return "default-src 'self'";
            return $"default-src 'self' {string.Join(" ", allowedSites)}";
        }
    }
}
