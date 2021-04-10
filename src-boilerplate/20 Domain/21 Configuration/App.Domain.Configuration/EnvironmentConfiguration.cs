using Signals.Aspects.Configuration;

namespace App.Domain.Configuration
{
    public class EnvironmentConfiguration : BaseConfiguration<EnvironmentConfiguration>
    {
        /// <summary>
        /// Represents the confgiruation section name of the custom configuration
        /// </summary>
        public override string Key => nameof(DomainConfiguration);

        /// <summary>
        /// Environment name
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Is active environment development
        /// </summary>
        public static bool IsDevelopment => Instance.Environment == "development";

        /// <summary>
        /// Is active environment test
        /// </summary>
        public static bool IsTest => Instance.Environment == "test";

        /// <summary>
        /// Is active environment production
        /// </summary>
        public static bool IsProduction => Instance.Environment == "production";
    }
}