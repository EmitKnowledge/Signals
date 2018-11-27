using System.Configuration;

namespace App.Client.Web.Configuration.Url
{
    public sealed class SslConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// SSL url port
        /// </summary>
        [ConfigurationProperty("port", IsKey = false, IsRequired = true)]
        public int Port
        {
            get
            {
                return (int)base["port"];
            }
            set
            {
                base["port"] = value;
            }
        }
    }
}
