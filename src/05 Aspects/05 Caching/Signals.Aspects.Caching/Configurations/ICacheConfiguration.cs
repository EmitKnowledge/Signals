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
        /// Provider that sets and get values
        /// </summary>
        IDataProvider DataProvider { get; set; }

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
