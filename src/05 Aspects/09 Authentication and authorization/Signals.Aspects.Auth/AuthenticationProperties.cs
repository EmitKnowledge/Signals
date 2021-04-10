using System;
using System.Collections.Generic;

namespace Signals.Aspects.Auth
{
    /// <summary>
    /// Default authentication properties
    /// </summary>
    public class AuthenticationProperties
    {
        /// <summary>
        /// State values about the authentication session
        /// </summary>
        public IDictionary<string, string> Dictionary { get; }

        /// <summary>
        /// Gets or sets whether the authentication session is persisted across multiple requests
        /// </summary>
        public bool IsPersistent { get; set; }

        /// <summary>
        /// Gets or sets the full path or absolute URI to be used as an http redirect response value
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the time at which the authentication ticket was issued
        /// </summary>
        public DateTimeOffset? IssuedUtc { get; set; }

        /// <summary>
        /// Gets or sets the time at which the authentication ticket expires
        /// </summary>
        public DateTimeOffset? ExpiresUtc { get; set; }

        /// <summary>
        /// Gets or sets if refreshing the authentication session should be allowed
        /// </summary>
        public bool? AllowRefresh { get; set; }
    }
}
