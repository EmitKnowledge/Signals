using Signals.Aspects.Caching.Configurations;
using Signals.Aspects.Caching.Entries;
using System;

namespace Signals.Aspects.Caching
{
    /// <summary>
    /// Cache
    /// </summary>
    public interface ICache
    {
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
    }
}
