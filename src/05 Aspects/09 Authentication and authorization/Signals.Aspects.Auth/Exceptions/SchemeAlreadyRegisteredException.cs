using System;

namespace Signals.Aspects.Auth.Exceptions
{
    /// <summary>
    /// Exception being thrown when same scheme name is used multiple times
    /// </summary>
    public class SchemeAlreadyRegisteredException : Exception
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="schemeName"></param>
        public SchemeAlreadyRegisteredException(string schemeName) : base($"Scheme {schemeName} is already registered")
        {
        }
    }
}
