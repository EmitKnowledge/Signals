using System.Configuration;
using App.Client.Web.Configuration.Analytics;
using App.Client.Web.Configuration.Content;
using App.Client.Web.Configuration.Url;
using App.Common.Base.Config;

namespace App.Client.Web.Configuration
{
    /// <summary>
    /// Represent the web configuration
    /// </summary>
    public class AppWebConfiguration : GenericConfigurationSection<AppWebConfiguration>
    {
        /// <summary>
        /// Represents the confgiruation section name of the custom configuration
        /// </summary>
        public override string ConfigurationName => @"App.Client.Web";

        /// <summary>
        /// Url for which this params are configured
        /// </summary>
        [ConfigurationProperty("url", IsKey = false, IsRequired = false)]
        public string Url
        {
            get
            {
                return (string)base["url"];
            }
            set
            {
                base["url"] = value;
            }
        }

        /// <summary>
        /// Analytics configuration section
        /// </summary>
        [ConfigurationProperty("analytics", IsRequired = true)]
        public AnalyticsConfigurationElement Analytics => (AnalyticsConfigurationElement)this["analytics"];

        /// <summary>
        /// CDN resources configuration section
        /// </summary>
        [ConfigurationProperty("staticResources", IsRequired = true)]
        public StaticResourcesConfigurationElement StaticResourcesConfiguration => (StaticResourcesConfigurationElement)this["staticResources"];

        /// <summary>
        /// SSL configuration section
        /// </summary>
        [ConfigurationProperty("sslConfiguration", IsRequired = true)]
        public SslConfigurationElement SslConfiguration => (SslConfigurationElement)this["sslConfiguration"];
    }
}
