using System.Configuration;
using App.Common.Configuration.Application;
using App.Common.Configuration.ExternalApis;
using App.Common.Configuration.Notifications;
using App.Common.Configuration.Security;
using App.Common.Configuration.Storage;
using App.Common.Configuration.Web;
using App.Common.Configuration.Database;
using Signals.Aspects.Configuration;

namespace App.Common.Configuration
{
    public class BusinessConfiguration : BaseConfiguration<BusinessConfiguration>
    {
        /// <summary>
        /// Represents the confgiruation section name of the custom configuration
        /// </summary>
        public override string Key => nameof(BusinessConfiguration);

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
