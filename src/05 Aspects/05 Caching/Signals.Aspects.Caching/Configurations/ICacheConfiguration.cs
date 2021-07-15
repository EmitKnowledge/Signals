using Signals.Aspects.Caching.Enums;
using System;

namespace Signals.Aspects.Caching.Configurations
{
    /// <summary>
    /// Cache configuration
    /// </summary>
    public interface ICacheConfiguration
    {
        /// <summary>
        /// Expiration time
        /// </summary>
        TimeSpan ExpirationTime { get; set; }

        /// <summary>
        /// Policy for expiration: 
        /// sliding, absolute
        /// </summary>
        CacheExpirationPolicy ExpirationPolicy { get; set; }
    }
}
