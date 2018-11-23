using Signals.Aspects.Caching.Enums;
using System;
using System.Runtime.Serialization;

namespace Signals.Aspects.Caching.Entries
{
    /// <summary>
    /// Common cache entity
    /// </summary>
    [Serializable, DataContract]
    public class CacheEntry
    {

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public CacheEntry(string key, object value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Key the cache is stored by
        /// </summary>
        [DataMember]
        public string Key { get; internal set; }

        /// <summary>
        /// Cached value
        /// </summary>
        [DataMember]
        public object Value { get; set; }

        /// <summary>
        /// Created on timestamp
        /// </summary>
        [DataMember]
        public DateTime CreatedOn { get; internal set; }

        /// <summary>
        /// Last accessed on timestamp
        /// </summary>
        [DataMember]
        public DateTime LastAccessedOn { get; internal set; }

        /// <summary>
        /// Time when entry expires
        /// </summary>
        [DataMember]
        public TimeSpan? ExpirationTime { get; internal set; }

        /// <summary>
        /// Expiration policy
        /// </summary>
        [DataMember]
        public CacheExpirationPolicy? ExpirationPolicy { get; internal set; }

        /// <summary>
        /// Cache collection
        /// </summary>
        private ICache _cache;

        /// <summary>
        /// Cache collecton
        /// </summary>
        public ICache Cache { get => _cache; set { _cache = value; PopulateEmptyPropertiesFromConfig(); } }

        private void PopulateEmptyPropertiesFromConfig()
        {
            ExpirationTime = ExpirationTime ?? Cache.Configuration.ExpirationTime;
            ExpirationPolicy = ExpirationPolicy ?? Cache.Configuration.ExpirationPolicy;
        }

        /// <summary>
        /// Callback when entry is accesed
        /// </summary>
        public virtual void InvokeGet()
        {
            LastAccessedOn = DateTime.UtcNow;
        }

        /// <summary>
        /// Callback when entry is set
        /// </summary>
        public virtual void InvokeSet()
        {
            CreatedOn = DateTime.UtcNow;
            LastAccessedOn = DateTime.UtcNow;
        }

        /// <summary>
        /// Callback when entry is invalidated
        /// </summary>
        public virtual void InvokeInvalidate()
        {
            Cache.Configuration.DataProvider.Remove(Key);
        }

        /// <summary>
        /// Callback when entry expired
        /// </summary>
        public virtual void InvokeExpire()
        {
            Cache.InvokeExpireCallbacks(Key);
            Cache.Configuration.DataProvider.Remove(Key);
        }

        /// <summary>
        /// Helper function to check if the entry is expired
        /// </summary>
        /// <returns></returns>
        public virtual bool IsExpired()
        {
            return ExpirationDate() < DateTime.UtcNow;
        }

        /// <summary>
        /// Helper function to calculate expiration date
        /// </summary>
        /// <returns></returns>
        public virtual DateTime ExpirationDate()
        {
            if (ExpirationPolicy != null && ExpirationPolicy.Value == CacheExpirationPolicy.Absolute)
            {
                if (ExpirationTime != null) return CreatedOn + ExpirationTime.Value;
            }

            if (ExpirationPolicy != null && ExpirationPolicy.Value == CacheExpirationPolicy.Sliding)
            {
                if (ExpirationTime != null) return LastAccessedOn + ExpirationTime.Value;
            }

            return DateTime.MinValue;
        }
    }
}