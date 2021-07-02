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
        public TimeSpan? ExpirationTime { get; set; }

        /// <summary>
        /// Expiration policy
        /// </summary>
        [DataMember]
        public CacheExpirationPolicy? ExpirationPolicy { get; set; }

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