using System.ComponentModel.DataAnnotations;

namespace App.Domain.Configuration.Notifications
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
        /// To conneciton string
        /// </summary>
        public string ConnectionString => $"Endpoint={Namespace};SharedAccessKeyName={Issuer};SharedAccessKey={IssuerKey}";

        /// <summary>
        /// CTOR
        /// </summary>
        public NotificationConfigurationElement()
        {
            IsEnabled = true;
        }
    }
}