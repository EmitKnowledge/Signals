using Signals.Aspects.Configuration;
using Signals.Aspects.Configuration.Validations;
using System.ComponentModel.DataAnnotations;

namespace Signals.Tests.Configuration.CustomConfigurations.AzureKeyVault
{
    public class SecretsConfiguration : BaseConfiguration<SecretsConfiguration>
    {
        public override string Key => nameof(SecretsConfiguration);

        [ValidateObject]
        public SmtpSecrets Smtp { get; set; }

        [Required(ErrorMessage = "API key is required.")]
        public string ApiKey { get; set; }

        public int RetryCount { get; set; } = 3;

        public float TimeoutSeconds { get; set; } = 1.5f;

        public bool Enabled { get; set; } = true;
    }
}
