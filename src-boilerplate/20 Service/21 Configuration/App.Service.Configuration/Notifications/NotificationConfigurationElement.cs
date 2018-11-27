using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace App.Common.Configuration.Notifications
{
    /// <summary>
    /// Windows azure service bus configuration
    /// </summary>
    public sealed class NotificationConfigurationElement
    {
        /// <summary>
        /// Service Bus application namespace
        /// </summary>
        [Required]
        public string Namespace { get; set; }

        /// <summary>
        /// Issuer value
        /// </summary>
        [Required]
        public string Issuer { get; set; }

        /// <summary>
        /// Issuer secret key
        /// </summary>
        [Required]
        public string IssuerKey { get; set; }

        /// <summary>
        /// Indicates that notification layer is on/off
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public NotificationConfigurationElement()
        {
            IsEnabled = true;
        }
    }
}
