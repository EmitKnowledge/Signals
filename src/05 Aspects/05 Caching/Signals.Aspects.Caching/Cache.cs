using Signals.Aspects.Caching.Configurations;
using Signals.Aspects.Caching.Entries;
using System;

namespace Signals.Aspects.Caching
{
    /// <summary>
    /// Base cache implementation
    /// </summary>
    public abstract class Cache : ICache
    {
        /// <summary>
        /// Cache configuration
        /// </summary>
        public ICacheConfiguration Configuration { get; set; }

        /// <summary>
        /// Cache value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void Set<T>(string key, T value)
        {
            var entry = Configuration.DataProvider.Set(key, value, this);
            entry.InvokeSet();
        }

        /// <summary>
        /// Cache value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime"></param>
        public virtual void Set<T>(string key, T value, TimeSpan expirationTime)
        {
            var entry = Configuration.DataProvider.Set(key, value, this);
            entry.ExpirationTime = expirationTime;
            entry.InvokeSet();
        }

        /// <summary>
        /// Cache value
        /// </summary>
        /// <param name="entry"></param>
        public virtual void Set(CacheEntry entry)
        {
            entry.Cache = this;
            entry = Configuration.DataProvider.Set(entry);
            entry.InvokeSet();
        }

        /// <summary>
        /// Get cached value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T Get<T>(string key) where T : class
        {
            var entry = InternalGet(key, false);
            return (T)entry?.Value;
        }

        /// <summary>
        /// Get cached value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheEntry Get(string key)
        {
            return InternalGet(key, false);
        }

        /// <summary>
        /// Get cached value while with expiring check
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isExpireInvoked"></param>
        /// <returns></returns>
        private CacheEntry InternalGet(string key, bool isExpireInvoked)
        {
            var entry = Configuration.DataProvider.Get(key);

            if (entry == null) return null;
            if (entry.IsExpired())
            {
	            if (isExpireInvoked) return null;
	            entry.InvokeExpire();
	            return InternalGet(key, true);

            }

            entry.InvokeGet();
            return entry;
        }

        /// <summary>
        /// Invalidate cache entry
        /// </summary>
        /// <param name="key"></param>
        public virtual void Invalidate(string key)
        {
            var entry = Configuration.DataProvider.Get(key);
            entry.InvokeInvalidate();
        }

        /// <summary>
        /// Invalidate all cache entries
        /// </summary>
        public virtual void Invalidate()
        {
            var all = Configuration.DataProvider.GetAll();
			foreach (var entry in all)
			{
				Invalidate(entry.Key);
			}
        }
        
        /// <summary>
        /// Invokes callbacks on expiration
        /// </summary>
        /// <param name="key"></param>
        public abstract void InvokeExpireCallbacks(string key);

        /// <summary>
        /// Registers global callback when any entry expires
        /// </summary>
        /// <param name="action"></param>
        public abstract void OnExpire(Action<CacheEntry> action);

	    /// <summary>
	    /// Registers callback when an entry with corresponding key expires
	    /// </summary>
	    /// <param name="key"></param>
	    /// <param name="action"></param>
	    public abstract void OnExpire(string key, Action<CacheEntry> action);
    }
}
