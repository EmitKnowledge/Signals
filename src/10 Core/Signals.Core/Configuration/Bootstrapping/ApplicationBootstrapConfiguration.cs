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
using Signals.Core.Processes.Recurring;
using Signals.Core.Processes.Recurring.Logging;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Signals.Core.Configuration.Bootstrapping
{
	/// <summary>
	/// Aspects configuration
	/// </summary>
	public abstract class ApplicationBootstrapConfiguration
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
		/// Synchronization logging provider
		/// </summary>
		protected IRecurringTaskLogProvider RecurringTaskLogProvider { get; set; }

		/// <summary>
		/// All loaded Signals types
		/// </summary>
		private List<Type> _allTypes;

		/// <summary>
		/// Build instances from configurations by convention
		/// </summary>
		/// <returns></returns>
		protected virtual IServiceContainer Resolve(params Assembly[] scanAssemblies)
		{
			scanAssemblies = scanAssemblies ?? new Assembly[0];
			_allTypes = scanAssemblies.SelectMany(assembly => assembly.LoadAllTypesFromAssembly().Where(type => type.FullName.StartsWith("Signals"))).ToList();

			var config = new ConfigurationBootstrapper();

			config.JsonSerializerSettings = () => JsonSerializerSettings;
			config.RecurringTaskLogProvider = () => RecurringTaskLogProvider;
            config.DependencyResolver = () => RegistrationService;
			config.Logging = () => GetInstance<ILogger>(LoggerConfiguration);
			config.Auditing = () => GetInstance<IAuditProvider>(AuditingConfiguration);
			config.Cache = () => GetInstance<ICache>(CacheConfiguration);
			config.Storage = () => GetInstance<IStorageProvider>(StorageConfiguration);
			config.MessageChannel = () => GetInstance<IMessageChannel>(ChannelConfiguration);
			config.PermissionProvider = () => GetInstance<IPermissionProvider>(SecurityConfiguration);
			config.AuthenticationManager = () => GetImplementationTypes<IAuthenticationManager>().SingleOrDefault();
			config.AuthorizationManager = () => GetImplementationTypes<IAuthorizationManager>().SingleOrDefault();
			config.TaskRegistry = () => TaskRegistry;
			config.ErrorHandling = () => StrategyBuilder;

			var localizaitonProvider = GetInstance<ILocalizationDataProvider>(LocalizationConfiguration);
			config.Localization = () => !localizaitonProvider.IsNull() ? new LocalizationProvider(localizaitonProvider) : null;

			if (SecurityConfiguration.IsNull())
			{
                if (config.AuthenticationManager()?.IsNull() == false)
                {
                    config.PermissionManager = () =>
                        GetImplementationTypes<IPermissionManager>()
                            .SingleOrDefault(x => !x.IsAssignableFrom(typeof(Processing.Authorization.PermissionManager)));
                }
			}
			else
			{
				config.PermissionManager = () => typeof(Processing.Authorization.PermissionManager);
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
				catch
				{
				}
			}

			return null;
		}
	}
}