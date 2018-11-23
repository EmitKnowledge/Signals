using System;

namespace Signals.Core.Web.Behaviour
{
    /// <summary>
    /// Cache location
    /// </summary>
    public enum CacheLocation
    {
        None,
        Client,
        Server,
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
