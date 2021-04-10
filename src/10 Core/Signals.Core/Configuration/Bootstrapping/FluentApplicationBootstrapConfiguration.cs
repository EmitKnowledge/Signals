using Newtonsoft.Json;
using Signals.Aspects.Auditing.Configurations;
using Signals.Aspects.BackgroundProcessing;
using Signals.Aspects.Benchmarking.Configurations;
using Signals.Aspects.Caching.Configurations;
using Signals.Aspects.CommunicationChannels.Configurations;
using Signals.Aspects.DI;
using Signals.Aspects.ErrorHandling;
using Signals.Aspects.Localization;
using Signals.Aspects.Logging.Configurations;
using Signals.Aspects.Security.Configurations;
using Signals.Aspects.Storage.Configurations;

namespace Signals.Core.Configuration.Bootstrapping
{
    /// <summary>
    /// Aspects configuration
    /// </summary>
    public abstract class FluentApplicationBootstrapConfiguration : ApplicationBootstrapConfiguration
    {
        /// <summary>
        /// Serialization settings
        /// </summary>
        public FluentApplicationBootstrapConfiguration AddJsonSerializerSettings(JsonSerializerSettings jsonSerializerSettings)
        {
            JsonSerializerSettings = jsonSerializerSettings;
            return this;
        }

        /// <summary>
        /// DI registration service
        /// </summary>
        public FluentApplicationBootstrapConfiguration AddRegistrationService(IRegistrationService registrationService)
        {
            RegistrationService = registrationService;
            return this;
        }

        /// <summary>
        /// Logger configuration
        /// </summary>
        public FluentApplicationBootstrapConfiguration AddLoggerConfiguration(ILoggingConfiguration loggerConfiguration)
        {
            LoggerConfiguration = loggerConfiguration;
            return this;
        }

        /// <summary>
        /// Auditing configuration
        /// </summary>
        public FluentApplicationBootstrapConfiguration AddAuditingConfiguration(IAuditingConfiguration auditingConfiguration)
        {
            AuditingConfiguration = auditingConfiguration;
            return this;
        }

        /// <summary>
        /// Cache configuration
        /// </summary>
        public FluentApplicationBootstrapConfiguration AddCacheConfiguration(ICacheConfiguration cacheConfiguration)
        {
            CacheConfiguration = cacheConfiguration;
            return this;
        }

        /// <summary>
        /// Localization configuration
        /// </summary>
        public FluentApplicationBootstrapConfiguration AddLocalizationConfiguration(ILocalizationConfiguration localizationConfiguration)
        {
            LocalizationConfiguration = localizationConfiguration;
            return this;
        }

        /// <summary>
        /// Storage configuration
        /// </summary>
        public FluentApplicationBootstrapConfiguration AddStorageConfiguration(IStorageConfiguration storageConfiguration)
        {
            StorageConfiguration = storageConfiguration;
            return this;
        }

        /// <summary>
        /// Communication channel configuration
        /// </summary>
        public FluentApplicationBootstrapConfiguration AddChannelConfiguration(IChannelConfiguration channelConfiguration)
        {
            ChannelConfiguration = channelConfiguration;
            return this;
        }

        /// <summary>
        /// Background tasks registry
        /// </summary>
        public FluentApplicationBootstrapConfiguration AddTaskRegistry(ITaskRegistry taskRegistry)
        {
            TaskRegistry = taskRegistry;
            return this;
        }

        /// <summary>
        /// Error handling strategy builder
        /// </summary>
        public FluentApplicationBootstrapConfiguration AddStrategyBuilder(IStrategyBuilder strategyBuilder)
        {
            StrategyBuilder = strategyBuilder;
            return this;
        }

        /// <summary>
        /// Security configuration
        /// </summary>
        public FluentApplicationBootstrapConfiguration AddSecurityConfiguration(ISecurityConfiguration securityConfiguration)
        {
            SecurityConfiguration = securityConfiguration;
            return this;
        }

        /// <summary>
        /// Benchmarking configuration
        /// </summary>
        public FluentApplicationBootstrapConfiguration AddBenchmarkingConfiguration(IBenchmarkingConfiguration benchmarkingConfiguration)
        {
            BenchmarkingConfiguration = benchmarkingConfiguration;
            return this;
        }
    }
}