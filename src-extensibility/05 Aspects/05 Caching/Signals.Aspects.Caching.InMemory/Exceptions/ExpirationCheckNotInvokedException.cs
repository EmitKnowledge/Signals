using System;

namespace Signals.Aspects.Caching.InMemory.Exceptions
{
    /// <summary>
    /// Expiration check is not started
    /// </summary>
    public class ExpirationCheckNotInvokedException : Exception
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public ExpirationCheckNotInvokedException() : base("Expiration check is not invoked.")
        {
        }
    }
}
