using Signals.Aspects.Caching.Entries;
using System.Collections.Generic;

namespace Signals.Aspects.Caching
{
    /// <summary>
    /// Data provider
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Gets cached entry
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        CacheEntry Get(string key);

        /// <summary>
        /// Removes cached entry
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        CacheEntry Remove(string key);

        /// <summary>
        /// Gets all cached entries
        /// </summary>
        /// <returns></returns>
        IEnumerable<CacheEntry> GetAll();

        /// <summary>
        /// Sets cache entry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        CacheEntry Set(CacheEntry entry);

        /// <summary>
        /// Sets cache entry
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        CacheEntry Set(string key, object value, Cache cache);
    }
}
