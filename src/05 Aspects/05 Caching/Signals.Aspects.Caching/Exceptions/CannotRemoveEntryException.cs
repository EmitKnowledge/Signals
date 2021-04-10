using System;

namespace Signals.Aspects.Caching.Exceptions
{
    /// <summary>
    /// Cannot remove value from cache
    /// </summary>
    public class CannotRemoveEntryException : Exception
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="key"></param>
        public CannotRemoveEntryException(string key) : base($"Cannot remove value for key {key}.")
        {
        }
    }
}
