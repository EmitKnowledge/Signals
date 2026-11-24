using Signals.Aspects.Configuration;
using System;
using System.ComponentModel.DataAnnotations;

namespace Signals.Tests.Configuration.CustomConfigurations.AzureKeyVault
{
    public class CustomSecretsConfiguration : BaseConfiguration<CustomSecretsConfiguration>
    {
        public override string Key => "CustomKey";

        [Required]
        public string ApiKey { get; set; }

        public string ReadOnlyValue => "read-only";

        public string PrivateSetValue { get; private set; } = "private-set";

        public static string StaticValue { get; set; } = "static";

        public string this[int index]
        {
            get => "indexed";
            set => throw new InvalidOperationException("Indexers must not be populated from secrets.");
        }
    }
}
