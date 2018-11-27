using System.Configuration;

namespace App.Client.Web.Configuration.Analytics
{
    public sealed class AnalyticsConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Indicate if analytics traking should be enabled
        /// </summary>
        [ConfigurationProperty("enableAnalytics", IsKey = false, IsRequired = true)]
        public bool EnableAnalytics
        {
            get
            {
                return (bool)base["enableAnalytics"];
            }
            set
            {
                base["enableAnalytics"] = value;
            }
        }
    }
}
