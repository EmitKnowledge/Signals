﻿using Signals.Aspects.Caching.Entries;
using Signals.Aspects.Caching.InMemory.Configurations;
using System;
using System.Collections.Concurrent;

namespace Signals.Aspects.Caching.InMemory
{
    /// <summary>
    /// In memory cache implementation
    /// </summary>
    public class InMemoryCache : ICache
    {
        /// <summary>
        /// All entries
        /// </summary>
        private readonly ConcurrentDictionary<string, CacheEntry> Entries;

        /// <summary>
        /// Cache configuration
        /// </summary>
        private readonly InMemoryCacheConfiguration Configuration;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public InMemoryCache(InMemoryCacheConfiguration configuration)
        {
            Entries = new ConcurrentDictionary<string, CacheEntry>();
            Configuration = configuration;
        }

        /// <summary>
        /// Cache value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set<T>(string key, T value)
        {
            var entry = new CacheEntry(key, value);
            Set(entry);
        }

        /// <summary>
        /// Cache value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime"></param>
        public void Set<T>(string key, T value, TimeSpan expirationTime)
        {
            var entry = new CacheEntry(key, value);
            entry.ExpirationTime = expirationTime;
            Set(entry);
        }

        /// <summary>
        /// Cache value
        /// </summary>
        /// <param name="entry"></param>
        public void Set(CacheEntry entry)
        {
            entry.InvokeSet();

            entry.ExpirationTime = entry.ExpirationTime ?? Configuration.ExpirationTime;
            entry.ExpirationPolicy = entry.ExpirationPolicy ?? Configuration.ExpirationPolicy;

            if (!Entries.TryAdd(entry.Key, entry))
                Entries[entry.Key] = entry;
        }

        /// <summary>
        /// Get cached value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            return (T)Get(key)?.Value;
        }

        /// <summary>
        /// Get cached value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheEntry Get(string key)
        {
            var entry = Entries.TryGetValue(key, out var value) ? value : null;

            if (entry == null) return null;

            entry.InvokeGet();
            if (!entry.IsExpired()) return entry;
            Invalidate(key);

            return null;
        }

        /// <summary>
        /// Invalidate cache entry
        /// </summary>
        /// <param name="key"></param>
        public void Invalidate(string key)
        {
            var entry = Get(key);
            if (entry == null) return;
            Entries.TryRemove(key, out var _);
        }
    }
}
