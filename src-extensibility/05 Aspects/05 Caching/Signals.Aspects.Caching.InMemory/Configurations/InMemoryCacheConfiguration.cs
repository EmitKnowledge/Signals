using Signals.Aspects.Caching.Configurations;
using Signals.Aspects.Caching.Enums;
using System;

namespace Signals.Aspects.Caching.InMemory.Configurations
{
    /// <summary>
    /// In memory cache configuration
    /// </summary>
    public class InMemoryCacheConfiguration : ICacheConfiguration
    {
        /// <summary>
        /// Expiration time
        /// </summary>
        public TimeSpan ExpirationTime { get; set; }

        /// <summary>
        /// Policy for expiration: 
        /// sliding, absolute
        /// </summary>
        public CacheExpirationPolicy ExpirationPolicy { get; set; }

	    /// <summary>
	    /// CTOR
	    /// </summary>
	    public InMemoryCacheConfiguration()
	    {
		    ExpirationTime = TimeSpan.FromMinutes(5);
		    ExpirationPolicy = CacheExpirationPolicy.Sliding;
	    }
	}
}
