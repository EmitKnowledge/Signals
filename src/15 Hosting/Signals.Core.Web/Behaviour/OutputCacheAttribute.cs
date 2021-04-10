using System;

namespace Signals.Core.Web.Behaviour
{
    /// <summary>
    /// Cache location
    /// </summary>
    public enum CacheLocation
    {
        /// <summary>
        /// Disable cache
        /// </summary>
        None,

        /// <summary>
        /// Store cache data on client side
        /// </summary>
        Client,

        /// <summary>
        /// Store cache data on server side
        /// </summary>
        Server,

        /// <summary>
        /// Store cache data on both client and server side
        /// </summary>
        ClientAndServer
    }

    /// <summary>
    /// Process cache control
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OutputCacheAttribute : Attribute
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public OutputCacheAttribute()
        {
            Duration = 5;
            Location = CacheLocation.Client;
            VaryByQueryParams = new string[0];
        }

        /// <summary>
        /// Cache duration in seconds
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Cache breaker parameter
        /// </summary>
        public string[] VaryByQueryParams { get; set; }

        /// <summary>
        /// Server or client cached
        /// </summary>
        public CacheLocation Location { get; set; }
    }
}
