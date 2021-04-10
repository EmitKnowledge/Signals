using System;

namespace Signals.Aspects.Caching.Exceptions
{
    /// <summary>
    /// Cannot set value in cache
    /// </summary>
    public class CannotCacheException : Exception
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="key"></param>
        public CannotCacheException(string key) : base($"Cannot cache value for key {key}.")
        {
        }
    }
}
