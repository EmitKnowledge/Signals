using Signals.Aspects.Configuration;
using Signals.Aspects.Configuration.Validations;
using System.ComponentModel.DataAnnotations;

namespace Signals.Clients.WebApi.Configuration
{
    /// <summary>
    /// Debug application secrets loaded from Azure Key Vault
    /// </summary>
    public class SecretsConfiguration : BaseConfiguration<SecretsConfiguration>
    {
        /// <summary>
        /// Configuration key
        /// </summary>
        public override string Key => nameof(SecretsConfiguration);

        /// <summary>
        /// SMTP account secrets
        /// </summary>
        [ValidateObject]
        public SmtpSecrets Smtp { get; set; }

        /// <summary>
        /// Service Bus secrets
        /// </summary>
        [ValidateObject]
        public ServiceBusSecrets ServiceBus { get; set; }

        /// <summary>
        /// Represents the api key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Resents a secret number
        /// </summary>
        public int Number { get; set; }
    }

    /// <summary>
    /// SMTP account secrets.
    /// </summary>
    public class SmtpSecrets
    {
        /// <summary>
        /// SMTP account username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// SMTP account password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }

    /// <summary>
    /// Service Bus secrets.
    /// </summary>
    public class ServiceBusSecrets
    {
        /// <summary>
        /// Service Bus connection string
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
