using Signals.Aspects.Caching.Configurations;
using Signals.Aspects.Caching.Enums;
using StackExchange.Redis;
using System;

namespace Signals.Aspects.Caching.Redis.Configurations
{
    /// <summary>
    /// In memory cache configuration
    /// </summary>
    public class RedisCacheConfiguration : ICacheConfiguration
    {
        public ConfigurationOptions ConfigurationOptions { get; set; }

        public string ConnectionEndpoint { get; set; }

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
	    public RedisCacheConfiguration()
	    {
		    ExpirationTime = TimeSpan.FromMinutes(5);
		    ExpirationPolicy = CacheExpirationPolicy.Sliding;
	    }
	}
}
