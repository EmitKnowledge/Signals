using Signals.Aspects.Caching.Entries;
using Signals.Aspects.Caching.Exceptions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Aspects.Caching.InMemory
{
    /// <summary>
    /// In memory data provider
    /// </summary>
    public class InMemoryDataProvider : IDataProvider
    {
        /// <summary>
        /// All entries
        /// </summary>
        private readonly ConcurrentDictionary<string, CacheEntry> _entries;

        /// <summary>
        /// CTOR
        /// </summary>
        public InMemoryDataProvider()
        {
            _entries = new ConcurrentDictionary<string, CacheEntry>();
        }

        /// <summary>
        /// Gets cached entry
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheEntry Get(string key)
        {
	        return _entries.TryGetValue(key, out var value) ? value : null;
        }

        /// <summary>
        /// Gets all cached entries
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CacheEntry> GetAll()
        {
            return _entries.Values.ToList();
        }

        /// <summary>
        /// Removes cached entry
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheEntry Remove(string key)
        {
            if (_entries.TryRemove(key, out var value)) return value;
            throw new CannotRemoveEntryException(key);
        }

        /// <summary>
        /// Sets cache entry
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public CacheEntry Set(string key, object value, Cache cache)
        {
            var entry = new CacheEntry(key, value);
            entry.Cache = cache;
            if (_entries.TryAdd(key, entry)) return entry;
            throw new CannotRemoveEntryException(key);
        }

        /// <summary>
        /// Sets cache entry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public CacheEntry Set(CacheEntry entry)
        {
            if (!_entries.ContainsKey(entry.Key))
            {
                if (_entries.TryAdd(entry.Key, entry)) return entry;
                throw new CannotCacheException(entry.Key);
            }

	        _entries[entry.Key] = entry;
	        return entry;
		}
    }
}
