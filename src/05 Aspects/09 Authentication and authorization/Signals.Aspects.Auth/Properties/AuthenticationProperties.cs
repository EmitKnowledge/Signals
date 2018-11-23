using System;
using System.Collections.Generic;

namespace Signals.Aspects.Auth.Properties
{
    public class AuthenticationProperties
    {
        //
        // Summary:
        //     State values about the authentication session.
        public IDictionary<string, string> Dictionary { get; }
        //
        // Summary:
        //     Gets or sets whether the authentication session is persisted across multiple
        //     requests.
        public bool IsPersistent { get; set; }
        //
        // Summary:
        //     Gets or sets the full path or absolute URI to be used as an http redirect response
        //     value.
        public string RedirectUri { get; set; }
        //
        // Summary:
        //     Gets or sets the time at which the authentication ticket was issued.
        public DateTimeOffset? IssuedUtc { get; set; }
        //
        // Summary:
        //     Gets or sets the time at which the authentication ticket expires.
        public DateTimeOffset? ExpiresUtc { get; set; }
        //
        // Summary:
        //     Gets or sets if refreshing the authentication session should be allowed.
        public bool? AllowRefresh { get; set; }
    }
}
