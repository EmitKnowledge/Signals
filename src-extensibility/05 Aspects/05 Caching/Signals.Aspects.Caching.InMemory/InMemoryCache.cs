using Signals.Aspects.Caching.Configurations;
using Signals.Aspects.Caching.Entries;
using Signals.Aspects.Caching.InMemory.Configurations;
using System;
using System.Collections.Concurrent;

namespace Signals.Aspects.Caching.InMemory
{
    /// <summary>
    /// In memory cache implementation
    /// </summary>
    public class InMemoryCache : Cache, ICache
    {
        /// <summary>
        /// Entry specific expiration callbacks
        /// </summary>
        protected ConcurrentDictionary<string, ConcurrentBag<Action<CacheEntry>>> EntryExpirationCallbacks { get; set; }

        /// <summary>
        /// Global expiration callbacks
        /// </summary>
        protected ConcurrentBag<Action<CacheEntry>> GlobalExpirationCallbacks { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public InMemoryCache(ICacheConfiguration configuration = null)
        {
            EntryExpirationCallbacks = new ConcurrentDictionary<string, ConcurrentBag<Action<CacheEntry>>>();
            GlobalExpirationCallbacks = new ConcurrentBag<Action<CacheEntry>>();
            Configuration = configuration ?? new InMemoryCacheConfiguration();
        }

        /// <summary>
        /// Registers global callback when any entry expires
        /// </summary>
        /// <param name="action"></param>
        public override void OnExpire(Action<CacheEntry> action)
        {
            GlobalExpirationCallbacks.Add(action);
        }

        /// <summary>
        /// Registers callback when an entry with corresponding key expires
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public override void OnExpire(string key, Action<CacheEntry> action)
        {
            var hasEntry = EntryExpirationCallbacks.TryGetValue(key, out ConcurrentBag<Action<CacheEntry>> callbacks);

            if (!hasEntry)
            {
                callbacks = new ConcurrentBag<Action<CacheEntry>>();
                EntryExpirationCallbacks.TryAdd(key, callbacks);
            }

            callbacks.Add(action);
        }

        /// <summary>
        /// Invokes callbacks on expiration
        /// </summary>
        /// <param name="key"></param>
        public override void InvokeExpireCallbacks(string key)
        {
            var entry = Configuration.DataProvider.Get(key);

			foreach (var globalCallback in GlobalExpirationCallbacks)
			{
				globalCallback(entry);
			}

	        if (!EntryExpirationCallbacks.ContainsKey(key)) return;

	        foreach (var entryCallback in EntryExpirationCallbacks[key])
			{
				entryCallback(entry);
			}
        }
    }
}
