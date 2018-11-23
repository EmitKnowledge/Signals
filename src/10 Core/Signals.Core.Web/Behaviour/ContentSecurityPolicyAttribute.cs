using Signals.Core.Common.Instance;
using System;

namespace Signals.Core.Web.Behaviour
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ContentSecurityPolicyAttribute : ResponseHeaderAttribute
    {
        public ContentSecurityPolicyAttribute() : base("Content-Security-Policy", GenerateValue(null))
        {

        }

        public ContentSecurityPolicyAttribute(params string[] allowedSites) : base("Content-Security-Policy", GenerateValue(allowedSites))
        {
        }

        private static string GenerateValue(string[] allowedSites)
        {
            if (allowedSites.IsNullOrHasZeroElements()) return "default-src 'self'";
            return $"default-src 'self' {string.Join(" ", allowedSites)}";
        }
    }
}
