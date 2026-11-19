using Signals.Aspects.Configuration;

namespace Signals.Tests.Configuration.CustomConfigurations.AzureKeyVault
{
    public class OptionalSecretsConfiguration : BaseConfiguration<OptionalSecretsConfiguration>
    {
        public override string Key => nameof(OptionalSecretsConfiguration);

        public string ApiKey { get; set; } = "fallback-api-key";

        public int RetryCount { get; set; } = 3;
    }
}
