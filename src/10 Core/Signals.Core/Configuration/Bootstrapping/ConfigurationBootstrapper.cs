using Signals.Aspects.Auditing;
using Signals.Aspects.Auth;
using Signals.Aspects.BackgroundProcessing;
using Signals.Aspects.DI;
using Signals.Aspects.Caching;
using Signals.Aspects.CommunicationChannels;
using Signals.Aspects.ErrorHandling;
using Signals.Aspects.Localization;
using Signals.Aspects.Logging;
using Signals.Aspects.Security;
using Signals.Aspects.Storage;
using Signals.Core.Processes;
using Signals.Core.Processes.Recurring.Logging;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Execution;
using System;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using Newtonsoft.Json;
using Signals.Core.Processing.Behaviour;
using Signals.Aspects.Benchmarking;
using Signals.Core.Common.Smtp;
using Signals.Core.Processes.Business;
using Signals.Core.Processes.Api;
using Signals.Core.Processes.Distributed;
using Signals.Core.Processes.Recurring;
using Signals.Core.Processes.Export;
using Signals.Core.Processes.Import;

namespace Signals.Core.Configuration.Bootstrapping
{
    /// <summary>
    /// Application configuration
    /// </summary>
    internal class ConfigurationBootstrapper
    {
        /// <summary>
        /// Dependency resolver provider
        /// </summary>
        public Func<IRegistrationService> DependencyResolver { get; set; }

        /// <summary>
        /// Logger provider
        /// </summary>
        public Func<ILogger> Logging { get; set; }

        /// <summary>
        /// Auditer provider
        /// </summary>
        public Func<IAuditProvider> Auditing { get; set; }

        /// <summary>
        /// Cache provider
        /// </summary>
        public Func<ICache> Cache { get; set; }

        /// <summary>
        /// Localizer provider
        /// </summary>
        public Func<ILocalizationProvider> Localization { get; set; }

        /// <summary>
        /// Storage provider
        /// </summary>
        public Func<IStorageProvider> Storage { get; set; }

        /// <summary>
        /// Messaging channel provider
        /// </summary>
        public Func<IMessageChannel> MessageChannel { get; set; }

        /// <summary>
        /// Authenticaiton manager provider
        /// </summary>
        public Func<Type> AuthenticationManager { get; set; }

        /// <summary>
        /// Authorization manager provider
        /// </summary>
        public Func<Type> AuthorizationManager { get; set; }

        /// <summary>
        /// Background task registry provider
        /// </summary>
        public Func<ITaskRegistry> TaskRegistry { get; set; }

        /// <summary>
        /// Error handler provider
        /// </summary>
        public Func<IStrategyBuilder> ErrorHandling { get; set; }

        /// <summary>
        /// Security permission provider
        /// </summary>
        public Func<IPermissionProvider> PermissionProvider { get; set; }

        /// <summary>
        /// Security permission manager
        /// </summary>
        public Func<Type> PermissionManager { get; set; }

        /// <summary>
        /// Benchmark engine
        /// </summary>
        public Func<IBenchmarker> Benchmarker { get; set; }

        /// <summary>
        /// Synchronization logging provider
        /// </summary>
        public Func<IRecurringTaskLogProvider> RecurringTaskLogProvider { get; set; }

        /// <summary>
        /// Json serialization settings provider
        /// </summary>
        public Func<JsonSerializerSettings> JsonSerializerSettings { get; set; }

        /// <summary>
        /// Register all aspects into dependency resolver
        /// </summary>
        /// <returns></returns>
        public IServiceContainer Bootstrap(params Assembly[] scanAssemblies)
        {
            // Proc config validation
            ApplicationConfiguration config = null;
            try
            {
	            config = ApplicationConfiguration.Instance;
	            this.D("Assigning an existing application configuration.");
            }
            catch(Exception ex)
            {
	            this.D($"Exception has occurred during setting up the existing application configuration. Exception: {ex.Message}");
            }
            finally
            {
	            if (config.IsNull())
	            {
		            var message =
			            "Signals.Core.Configuration.ApplicationConfiguration is not provided. Please use a configuration provider to provide configuration values!";

                    this.D(message);
                    throw new Exception(message);
	            }
            }

            if (DependencyResolver.IsNull() || DependencyResolver().IsNull())
            {
	            var message = "Dependency resolver hasn't been configured.";
                this.D(message);
	            throw new Exception(message);
            }

            var resolver = DependencyResolver();

            if (!Logging.IsNull() && !Logging().IsNull())
            {
	            resolver.Register(typeof(ILogger), Logging());
	            this.D("Registration service -> registered Logging.");
            }

            if (!Auditing.IsNull() && !Auditing().IsNull())
            {
	            resolver.Register(typeof(IAuditProvider), Auditing());
	            this.D("Registration service -> registered Auditing.");
            }

            if (!Cache.IsNull() && !Cache().IsNull())
            {
	            resolver.Register(typeof(ICache), Cache());
	            this.D("Registration service -> registered Cache.");
            }

            if (!Localization.IsNull() && !Localization().IsNull())
            {
	            resolver.Register(typeof(ILocalizationProvider), Localization());
	            this.D("Registration service -> registered Localization.");
            }

            if (!Storage.IsNull() && !Storage().IsNull())
            {
	            resolver.Register(typeof(IStorageProvider), Storage());
	            this.D("Registration service -> registered Storage.");
            }

            if (!MessageChannel.IsNull() && !MessageChannel().IsNull())
            {
	            resolver.Register(typeof(IMessageChannel), MessageChannel());
	            this.D("Registration service -> registered MessageChannel.");
            }

            if (!AuthenticationManager.IsNull() && !AuthenticationManager().IsNull())
            {
	            resolver.Register(typeof(IAuthenticationManager), AuthenticationManager());
	            this.D("Registration service -> registered AuthenticationManager.");
            }

            if (!AuthorizationManager.IsNull() && !AuthorizationManager().IsNull())
            {
	            resolver.Register(typeof(IAuthorizationManager), AuthorizationManager());
	            this.D("Registration service -> registered AuthorizationManager.");
            }

            if (!TaskRegistry.IsNull() && !TaskRegistry().IsNull())
            {
	            resolver.Register(typeof(ITaskRegistry), TaskRegistry());
	            this.D("Registration service -> registered TaskRegistry.");
            }

            if (!PermissionProvider.IsNull() && !PermissionProvider().IsNull())
            {
	            resolver.Register(typeof(IPermissionProvider), PermissionProvider());
	            this.D("Registration service -> registered PermissionProvider.");
            }

            if (!Benchmarker.IsNull() && !Benchmarker().IsNull())
            {
	            resolver.Register(typeof(IBenchmarker), Benchmarker());
	            this.D("Registration service -> registered Benchmarker.");
            }

            if (!PermissionManager.IsNull() && !PermissionManager().IsNull())
            {
	            resolver.Register(typeof(IPermissionManager), PermissionManager());
	            this.D("Registration service -> registered PermissionManager.");
            }
            
            resolver.Register<CriticalErrorCallbackManager>();
            this.D("Registration service -> registered CriticalErrorCallbackManager.");

            resolver.Register<IProcessFactory, ProcessFactory>();
            this.D("Registration service -> registered ProcessFactory.");

            resolver.Register<IProcessExecutor, ProcessExecutor>();
            this.D("Registration service -> registered ProcessExecutor.");

            resolver.Register<IBusinessProcessContext, BusinessProcessContext>();
            this.D("Registration service -> registered BusinessProcessContext.");

            resolver.Register<IApiProcessContext, ApiProcessContext>();
            this.D("Registration service -> registered ApiProcessContext.");

            resolver.Register<IDistributedProcessContext, DistributedProcessContext>();
            this.D("Registration service -> registered DistributedProcessContext.");

            resolver.Register<IFileExportProcessContext, FileExportProcessContext>();
            this.D("Registration service -> registered FileExportProcessContext.");

            resolver.Register<IFileImportProcessContext, FileImportProcessContext>();
            this.D("Registration service -> registered FileImportProcessContext.");

            resolver.Register<IRecurringProcessContext, RecurringProcessContext>();
            this.D("Registration service -> registered RecurringProcessContext.");

            resolver.Register<Mediator>();
            this.D("Registration service -> registered Mediator.");

            RegisterProcesses(resolver, scanAssemblies);
            RegisterErrorHandling(resolver);
            RegisterJsonSerializerSettings(resolver);
            RegisterSmtp(config, resolver);
            RegisterSyncLogProvider(resolver);

            var services = SystemBootstrapper.Init(resolver, scanAssemblies);

            return services;
        }

        private void RegisterProcesses(IRegistrationService resolver, params Assembly[] scanAssemblies)
        {
	        this.D($"Creating process repository for provided {scanAssemblies?.Length} assemblies.");
            var processRepo = new ProcessRepository(scanAssemblies);
            resolver.Register(processRepo);
            this.D("Registration service -> registered ProcessRepository.");

            var processes = processRepo.All();
	        processes.ForEach(resolver.Register);
	        this.D($"Registration service -> registered {processes.Count} processes.");
        }

        private void RegisterErrorHandling(IRegistrationService resolver)
        {
            if (!ErrorHandling.IsNull() && !ErrorHandling().IsNull())
            {
                resolver.Register(typeof(IStrategyBuilder), ErrorHandling());
                this.D("Registration service -> registered ErrorHandling strategy.");

                var handler = ErrorHandling().Build();

                if (!handler.IsNull())
                {
                    resolver.Register(typeof(IStrategyHandler), handler);
                    this.D("Registration service -> registered ErrorHandling strategy handler.");
                }
            }
        }

        private void RegisterJsonSerializerSettings(IRegistrationService resolver)
        {
            if (!JsonSerializerSettings.IsNull() && !JsonSerializerSettings().IsNull())
            {
                resolver.Register(typeof(JsonSerializerSettings), JsonSerializerSettings());
                JsonConvert.DefaultSettings = JsonSerializerSettings;
                this.D("Registration service -> registered JsonSerializerSettings.");
            }
        }

        private void RegisterSmtp(ApplicationConfiguration config, IRegistrationService resolver)
        {
            if (config?.SmtpConfiguration?.IsNull() == false)
            {
                var server = config.SmtpConfiguration.Server;
                var port = config.SmtpConfiguration.Port;
                var useSsl = config.SmtpConfiguration.UseSsl;
                var username = config.SmtpConfiguration.Username;
                var password = config.SmtpConfiguration.Password;

                var instance = new SmtpClient(server, port)
                {
                    EnableSsl = useSsl,
                    Credentials = new NetworkCredential(username, password)
                };

                var wrapper = new SmtpClientWrapper(instance);
                wrapper.WhitelistedEmails = config.WhitelistedEmails;
                wrapper.WhitelistedEmailDomains = config.WhitelistedEmailDomains;

                resolver.Register<ISmtpClient>(wrapper);
                resolver.Register<SmtpClient>(instance);
                this.D("Registration service -> registered SMTP client.");
            }
        }

        private void RegisterSyncLogProvider(IRegistrationService resolver)
        {
            if (!TaskRegistry.IsNull() && !TaskRegistry().IsNull())
            {
                if (RecurringTaskLogProvider.IsNull() || RecurringTaskLogProvider().IsNull())
                {
                    RecurringTaskLogProvider = () => new RecurringTaskLogProvider();
                    this.D("Registration service -> registered RecurringTaskLogProvider client.");
                }

                resolver.Register(RecurringTaskLogProvider);
            }
        }
    }
}