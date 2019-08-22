using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Core.Background.Configuration.ConfigurationSegments
{
    /// <summary>
    /// Notification configuration on background startup
    /// </summary>
    public class StartupNotificationConfiguration
    {
        /// <summary>
        /// Emails to be notified
        /// </summary>
        public List<string> Emails { get; set; }

        /// <summary>
        /// Email subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Email body
        /// </summary>
        public string Body { get; set; }
    }
}
