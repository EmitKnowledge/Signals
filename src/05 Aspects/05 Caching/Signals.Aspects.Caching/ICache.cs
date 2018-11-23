using Signals.Aspects.Caching.Configurations;
using Signals.Aspects.Caching.Entries;
using System;

namespace Signals.Aspects.Caching
{
    public interface ICache
    {
        /// <summary>
        /// Cache configuration
        /// </summary>
        ICacheConfiguration Configuration { get; set; }

        /// <summary>
        /// Cache value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Set<T>(string key, T value);

        /// <summary>
        /// Cache value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime"></param>
        void Set<T>(string key, T value, TimeSpan expirationTime);

        /// <summary>
        /// Cache value
        /// </summary>
        /// <param name="entry"></param>
        void Set(CacheEntry entry);

        /// <summary>
        /// Get cached value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// Get cached value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        CacheEntry Get(string key);

        /// <summary>
        /// Invalidate cache entry
        /// </summary>
        /// <param name="key"></param>
        void Invalidate(string key);

        /// <summary>
        /// Invalidate all cache entries
        /// </summary>
        void Invalidate();

        /// <summary>
        /// Invokes callbacks on expiration
        /// </summary>
        /// <param name="key"></param>
        void InvokeExpireCallbacks(string key);

        /// <summary>
        /// Registers global callback when any entry expires
        /// </summary>
        /// <param name="action"></param>
        void OnExpire(Action<CacheEntry> action);

	    /// <summary>
	    /// Registers callback when an entry with corresponding key expires
	    /// </summary>
	    /// <param name="key"></param>
	    /// <param name="action"></param>
	    void OnExpire(string key, Action<CacheEntry> action);
    }
}
