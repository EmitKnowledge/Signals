using App.Domain.Configuration.Application;
using App.Domain.Configuration.Database;
using App.Domain.Configuration.ExternalApis;
using App.Domain.Configuration.Notifications;
using App.Domain.Configuration.Security;
using App.Domain.Configuration.Storage;
using App.Domain.Configuration.Web;
using Signals.Aspects.Configuration;

namespace App.Domain.Configuration
{
    public class DomainConfiguration : BaseConfiguration<DomainConfiguration>
    {
        /// <summary>
        /// Represents the confgiruation section name of the custom configuration
        /// </summary>
        public override string Key => nameof(DomainConfiguration);

        /// <summary>
        /// Configuration for the application itself
        /// </summary>
        public ApplicationConfigurationElement ApplicationConfiguration { get; set; }

        /// <summary>
        /// Configuration of the security
        /// </summary>
        public SecurityConfigurationElement SecurityConfiguration { get; set; }

        /// <summary>
        /// Configuration for notification
        /// </summary>
        public NotificationConfigurationElement NotificationConfiguration { get; set; }

        /// <summary>
        /// Configuration for storage
        /// </summary>
        public StorageConfigurationElement StorageConfiguration { get; set; }

        /// <summary>
        /// External apis configuration
        /// </summary>
        public ExternalApisConfigurationElement ExternalApisConfiguration { get; set; }

        /// <summary>
        /// Configuration for web
        /// </summary>
        public WebConfigurationElement WebConfiguration { get; set; }

        /// <summary>
        /// Configuration for database
        /// </summary>
        public DatabaseConfigurationElement DatabaseConfiguration { get; set; }
    }
}