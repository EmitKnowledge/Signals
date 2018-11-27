using System.Configuration;

namespace App.Client.Web.Configuration.Content
{
    public sealed class StaticResourcesConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Cookieless cdn url. Used for serving static resources 
        /// </summary>
        [ConfigurationProperty("cdnUrl", IsKey = false, IsRequired = false)]
        public string CdnUrl
        {
            get
            {
                return (string)base["cdnUrl"];
            }
            set
            {
                base["cdnUrl"] = value;
            }
        }
    }
}
