using System;

namespace Signals.Aspects.Auth.Attributes
{
    /// <summary>
    /// Claim extraction attribute for describing a property as claim 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class ClaimTypeAttribute : Attribute
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="type"></param>
        public ClaimTypeAttribute(string type)
        {
            Type = type;
        }

        /// <summary>
        /// Claim key
        /// </summary>
        public string Type { get; }
    }
}
