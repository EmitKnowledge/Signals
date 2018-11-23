namespace Signals.Aspects.Caching.Enums
{
    /// <summary>
    /// Expiration policy
    /// </summary>
    public enum CacheExpirationPolicy
    {
        /// <summary>
        /// Expires after a timeout from the time the entry is created
        /// </summary>
        Absolute = 1,

        /// <summary>
        /// Expires after a timeout from the time the entry is last accessed
        /// </summary>
        Sliding = 2
    }
}
