using Signals.Aspects.Auditing;
using Signals.Aspects.Auditing.Configurations;
using Signals.Aspects.Auth;
using Signals.Aspects.BackgroundProcessing;
using Signals.Aspects.Caching;
using Signals.Aspects.Caching.Configurations;
using Signals.Aspects.CommunicationChannels;
using Signals.Aspects.CommunicationChannels.Configurations;
using Signals.Aspects.DI;
using Signals.Aspects.ErrorHandling;
using Signals.Aspects.Localization;
using Signals.Aspects.Localization.Base;
using Signals.Aspects.Logging;
using Signals.Aspects.Logging.Configurations;
using Signals.Aspects.Security;
using Signals.Aspects.Security.Configurations;
using Signals.Aspects.Storage;
using Signals.Aspects.Storage.Configurations;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Signals.Aspects.Benchmarking;
using Signals.Aspects.Benchmarking.Configurations;
using Signals.Core.Common.Serialization;

namespace Signals.Core.Configuration.Bootstrapping
{
    internal interface IApplicationBootstrapConfiguration
    {
        /// <summary>
        /// Serialization settings
        /// </summary>
        JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// DI registration service
        /// </summary>
        IRegistrationService RegistrationService { get; set; }

        /// <summary>
        /// Logger configuration
        /// </summary>
        ILoggingConfiguration LoggerConfiguration { get; set; }

        /// <summary>
        /// Auditing configuration
        /// </summary>
        IAuditingConfiguration AuditingConfiguration { get; set; }

        /// <summary>
        /// Cache configuration
        /// </summary>
        ICacheConfiguration CacheConfiguration { get; set; }

        /// <summary>
        /// Localization configuration
        /// </summary>
        ILocalizationConfiguration LocalizationConfiguration { get; set; }

        /// <summary>
        /// Storage configuration
        /// </summary>
        IStorageConfiguration StorageConfiguration { get; set; }

        /// <summary>
        /// Communication channel configuration
        /// </summary>
        IChannelConfiguration ChannelConfiguration { get; set; }

        /// <summary>
        /// Background tasks registry
        /// </summary>
        ITaskRegistry TaskRegistry { get; set; }

        /// <summary>
        /// Error handling strategy builder
        /// </summary>
        IStrategyBuilder StrategyBuilder { get; set; }

        /// <summary>
        /// Security configuration
        /// </summary>
        ISecurityConfiguration SecurityConfiguration { get; set; }

        /// <summary>
        /// Benchmarking configuration
        /// </summary>
        IBenchmarkingConfiguration BenchmarkingConfiguration { get; set; }

        /// <summary>
        /// Build instances from configurations by convention
        /// </summary>
        /// <returns></returns>
        IServiceContainer Bootstrap(params Assembly[] scanAssemblies);

        /// <summary>
        /// Build instances from configurations by convention
        /// </summary>
        /// <returns></returns>
        IServiceContainer Resolve(ConfigurationBootstrapper configurationBootstrapper = null, params Assembly[] scanAssemblies);
    }

    /// <summary>
    /// Aspects configuration
    /// </summary>
    public abstract class ApplicationBootstrapConfiguration : IApplicationBootstrapConfiguration
    {
        /// <summary>
        /// Serialization settings
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// DI registration service
        /// </summary>
        public IRegistrationService RegistrationService { get; set; }

        /// <summary>
        /// Logger configuration
        /// </summary>
        public ILoggingConfiguration LoggerConfiguration { get; set; }

        /// <summary>
        /// Auditing configuration
        /// </summary>
        public IAuditingConfiguration AuditingConfiguration { get; set; }

        /// <summary>
        /// Cache configuration
        /// </summary>
        public ICacheConfiguration CacheConfiguration { get; set; }

        /// <summary>
        /// Localization configuration
        /// </summary>
        public ILocalizationConfiguration LocalizationConfiguration { get; set; }

        /// <summary>
        /// Storage configuration
        /// </summary>
        public IStorageConfiguration StorageConfiguration { get; set; }

        /// <summary>
        /// Communication channel configuration
        /// </summary>
        public IChannelConfiguration ChannelConfiguration { get; set; }

        /// <summary>
        /// Background tasks registry
        /// </summary>
        public ITaskRegistry TaskRegistry { get; set; }

        /// <summary>
        /// Error handling strategy builder
        /// </summary>
        public IStrategyBuilder StrategyBuilder { get; set; }

        /// <summary>
        /// Security configuration
        /// </summary>
        public ISecurityConfiguration SecurityConfiguration { get; set; }

        /// <summary>
        /// Benchmarking configuration
        /// </summary>
        public IBenchmarkingConfiguration BenchmarkingConfiguration { get; set; }

        /// <summary>
        /// Synchronization logging provider
        /// </summary>
        //protected IRecurringTaskLogProvider RecurringTaskLogProvider { get; set; }

        /// <summary>
        /// All loaded Signals types
        /// </summary>
        private List<Type> _allTypes;

        /// <summary>
        /// Build instances from configurations by convention
        /// </summary>
        /// <returns></returns>
        public virtual IServiceContainer Bootstrap(params Assembly[] scanAssemblies)
        {
            this.D("Begin bootstrapping.");
            var serviceContainer = Resolve(scanAssemblies: scanAssemblies);
            this.D("Bootstrapping completed.");
            return serviceContainer;
        }
        
        /// <summary>
        /// Internal resolver
        /// </summary>
        /// <param name="configurationBootstrapper"></param>
        /// <param name="scanAssemblies"></param>
        /// <returns></returns>
        IServiceContainer IApplicationBootstrapConfiguration.Resolve(ConfigurationBootstrapper configurationBootstrapper, params Assembly[] scanAssemblies)
        {
            return Resolve(configurationBootstrapper, scanAssemblies);
        }

        /// <summary>
        /// Build instances from configurations by convention
        /// </summary>
        /// <returns></returns>
        internal virtual IServiceContainer Resolve(ConfigurationBootstrapper configurationBootstrapper = null, params Assembly[] scanAssemblies)
        {
	        ConfigurationBootstrapper config;
	        if (configurationBootstrapper == null)
	        {
		        this.D("Creating default bootstrapper configuration.");
		        config = new ConfigurationBootstrapper();
            }
	        else
	        {
		        this.D("Assigning an existing bootstrapper configuration.");
		        config = configurationBootstrapper;
	        }

	        if (scanAssemblies == null)
	        {
		        this.D("No assemblies have been provided. Assigning the caller assembly.");
		        scanAssemblies = new Assembly[0];
	        }
	        else
	        {
		        this.D($"Total {scanAssemblies.Length} assemblies have been provided.");
	        }

	        this.D("Loading all types under Signals.* namespace.");
            _allTypes = scanAssemblies.SelectMany(
	            assembly => assembly.LoadAllTypesFromAssembly()
					.Where(type => type != null && 
					               !string.IsNullOrEmpty(type.FullName) && 
					               type.FullName.StartsWith("Signals")))
	            .ToList();
            this.D($"Total {_allTypes.Count} types under Signals has been loaded.");

            config.JsonSerializerSettings = () => JsonSerializerSettings;
            this.D("Set default JsonSerializerSettings.");

            //config.RecurringTaskLogProvider = () => RecurringTaskLogProvider;
            //this.D("Set default RecurringTaskLogProvider.");

            config.DependencyResolver = () => RegistrationService;
            this.D($"Set DependencyResolver -> {RegistrationService?.GetType().FullName ?? "N/A"}.");

            config.Logging = () => GetInstance<ILogger>(LoggerConfiguration);
            this.D($"Set Logging -> {config.Logging()?.GetType().FullName ?? "N/A"} with configuration: {LoggerConfiguration?.SerializeJson() ?? "N/A"}.");

            config.Auditing = () => GetInstance<IAuditProvider>(AuditingConfiguration);
            this.D($"Set Auditing -> {config.Auditing()?.GetType().FullName ?? "N/A"} with configuration: {AuditingConfiguration?.SerializeJson() ?? "N/A"}.");

            config.Cache = () => GetInstance<ICache>(CacheConfiguration);
            this.D($"Set Cache -> {config.Cache()?.GetType().FullName ?? "N/A"} with configuration: {CacheConfiguration?.SerializeJson() ?? "N/A"}.");

            config.Storage = () => GetInstance<IStorageProvider>(StorageConfiguration);
            this.D($"Set Storage -> {config.Storage()?.GetType().FullName ?? "N/A"} with configuration: {StorageConfiguration?.SerializeJson() ?? "N/A"}.");

            config.MessageChannel = () => GetInstance<IMessageChannel>(ChannelConfiguration);
            this.D($"Set MessageChannel -> {config.MessageChannel()?.GetType().FullName ?? "N/A"} with configuration: {ChannelConfiguration?.SerializeJson() ?? "N/A"}.");

            config.PermissionProvider = () => GetInstance<IPermissionProvider>(SecurityConfiguration);
            this.D($"Set PermissionProvider -> {config.PermissionProvider()?.GetType().FullName ?? "N/A"} with configuration: {SecurityConfiguration?.SerializeJson() ?? "N/A"}.");

            config.Benchmarker = () => GetInstance<IBenchmarker>(BenchmarkingConfiguration);
            this.D($"Set Benchmarker -> {config.Benchmarker()?.GetType().FullName ?? "N/A"} with configuration: {BenchmarkingConfiguration?.SerializeJson() ?? "N/A"}.");

            config.AuthenticationManager = () => GetImplementationTypes<IAuthenticationManager>().SingleOrDefault();
            this.D($"Set AuthenticationManager -> {config.AuthenticationManager()?.GetType().FullName ?? "N/A"}.");

            config.AuthorizationManager = () => GetImplementationTypes<IAuthorizationManager>().SingleOrDefault();
            this.D($"Set AuthorizationManager -> {config.AuthorizationManager()?.GetType().FullName ?? "N/A"}.");

            config.TaskRegistry = () => TaskRegistry;
            this.D($"Set TaskRegistry -> {config.TaskRegistry()?.GetType().FullName ?? "N/A"}.");

            config.ErrorHandling = () => StrategyBuilder;
            this.D($"Set ErrorHandling -> {config.ErrorHandling()?.GetType().FullName ?? "N/A"}.");

            var localizationProvider = GetInstance<ILocalizationDataProvider>(LocalizationConfiguration);
            config.Localization = () => !localizationProvider.IsNull() ? new LocalizationProvider(localizationProvider) : null;
            this.D($"Set Localization -> {config.Localization()?.GetType().FullName ?? "N/A"} with configuration: {LocalizationConfiguration?.SerializeJson() ?? "N/A"}.");

            if (SecurityConfiguration.IsNull())
            {
	            this.D("No security configuration has been provided.");
                if (config.AuthenticationManager()?.IsNull() == false)
                {
	                this.D("AuthenticationManager exists.");
	                config.PermissionManager = () =>
		                GetImplementationTypes<IPermissionManager>()
			                .SingleOrDefault(x =>
				                !x.IsAssignableFrom(typeof(Processing.Authorization.PermissionManager)));
                    this.D($"Set PermissionManager -> {config.PermissionManager()?.GetType().FullName ?? "N/A"}.");
                }
            }
            else
            {
                config.PermissionManager = () => typeof(Processing.Authorization.PermissionManager);
                this.D($"Set PermissionManager -> {config.PermissionManager()?.GetType().FullName ?? "N/A"}.");
            }

            return config.Bootstrap(scanAssemblies);
        }

        /// <summary>
        /// Retrieves all implementations of a type
        /// </summary>
        /// <typeparam name="TDefinition"></typeparam>
        /// <returns></returns>
        private List<Type> GetImplementationTypes<TDefinition>() where TDefinition : class
        {
            var requiringType = typeof(TDefinition);
            var implementationTypes = _allTypes
                .Where(x => (x.GetInterfaces().Contains(requiringType) || x.IsSubclassOf(requiringType)) && !x.IsInterface && !x.IsAbstract)
                .Distinct()
                .ToList();
            return implementationTypes;
        }

        /// <summary>
        /// Create instance with arguments for TDefinition type
        /// </summary>
        /// <typeparam name="TDefinition"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        private TDefinition GetInstance<TDefinition>(params object[] args) where TDefinition : class
        {
            var implementationTypes = GetImplementationTypes<TDefinition>();
            if (implementationTypes.IsNullOrHasZeroElements()) return null;

            return GetInstance<TDefinition>(implementationTypes, args);
        }

        /// <summary>
        /// Create instance with arguments for TDefinition type
        /// </summary>
        /// <typeparam name="TDefinition"></typeparam>
        /// <param name="implementationTypes"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private TDefinition GetInstance<TDefinition>(List<Type> implementationTypes, params object[] args) where TDefinition : class
        {
            foreach (var type in implementationTypes)
            {
                try
                {
                    var instance = Activator.CreateInstance(type, args) as TDefinition;
                    return instance;
                }
                catch (Exception ex)
                {
                    this.D($"Exception has occurred while getting an instance for: {type.FullName}. Exception: {ex?.Message}.");
                }
            }

            return null;
        }
    }
}