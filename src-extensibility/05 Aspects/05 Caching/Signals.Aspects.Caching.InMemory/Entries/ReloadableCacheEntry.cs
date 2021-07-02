using Signals.Aspects.Caching.Entries;
using System;

namespace Signals.Aspects.Caching.InMemory.Entries
{
    /// <summary>
    /// Cached entity that reloads itself on expiry by a provider function
    /// </summary>
    public class ReloadableCacheEntry : CacheEntry
    {
        /// <summary>
        /// Provider function for value reloading 
        /// </summary>
        public Func<object> Provider { get; set; }

	    /// <summary>
	    /// CTOR
	    /// </summary>
	    /// <param name="key"></param>
	    /// <param name="provider"></param>
	    public ReloadableCacheEntry(string key, Func<object> provider) : base(key, null)
	    {
		    Value = provider(); Provider = provider;
	    }

	    /// <summary>
	    /// CTOR
	    /// </summary>
	    /// <param name="key"></param>
	    /// <param name="value"></param>
	    /// <param name="provider"></param>
	    public ReloadableCacheEntry(string key, object value, Func<object> provider) : base(key, value)
	    {
		    Provider = provider;
	    }

        /// <summary>
        /// Callback when entry is accesed
        /// </summary>
        public override void InvokeGet()
        {
            if (Value == null || IsExpired())
            {
                Value = Provider();
            }

            base.InvokeGet();
        }
    }
}
