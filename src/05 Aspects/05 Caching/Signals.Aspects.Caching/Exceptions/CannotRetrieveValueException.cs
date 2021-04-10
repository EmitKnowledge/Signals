using System;

namespace Signals.Aspects.Caching.Exceptions
{
    /// <summary>
    /// Cannot get value from cache
    /// </summary>
    public class CannotRetrieveValueException : Exception
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="key"></param>
        public CannotRetrieveValueException(string key) : base($"Cannot get value for key {key}.")
        {
        }
    }
}
