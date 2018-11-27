using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;

namespace App.Service.Configuration.ExternalApis
{
    /// <summary>
    /// Configuration for used external apis
    /// </summary>
    public sealed class ExternalApisConfigurationElement
    {
        public List<ExternalApiConfigurationElement> ExternalApisCollection { get; set; }

        public ExternalApisConfigurationElement()
        {
            ExternalApisCollection = new List<ExternalApiConfigurationElement>();
        }
    }

    /// <summary>
    /// Represent external api configuration element
    /// </summary>
    public sealed class ExternalApiConfigurationElement 
    {
        /// <summary>
        /// Default exnternal api name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// sid
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Application id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Application secret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// Client id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Client secret
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Secret Key
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Secret Word
        /// </summary>
        public string SecretWord { get; set; }

        /// <summary>
        /// Publishable Key
        /// </summary>
        public string PublishableKey { get; set; }

        /// <summary>
        /// Consumer key
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// Consumer secret
        /// </summary>
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// Access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Api key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Api secret
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// Private key
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Id of mailchimp list id
        /// </summary>
        public string ListId { get; set; }

        /// <summary>
        /// Twilio sms number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Indicate service sandbox mode
        /// </summary>
        public bool IsSandbox { get; set; }

        /// <summary>
        /// Payoneer partner id
        /// </summary>
        public string PartnerId { get; set; }
    }
}
