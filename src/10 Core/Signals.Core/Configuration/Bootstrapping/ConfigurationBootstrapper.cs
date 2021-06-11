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
using Signals.Core.Processes.Base;

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
            }
            catch { }
            finally
            {
                if (config.IsNull()) throw new Exception("Signals.Core.Configuration.ApplicationConfiguration is not provided. Please use a configuration provider to provide configuration values!");
            }

            if (DependencyResolver.IsNull() || DependencyResolver().IsNull()) throw new Exception("Dependency resolver not configured");

            var resolver = DependencyResolver();

            if (!Logging.IsNull() && !Logging().IsNull()) resolver.Register(typeof(ILogger), Logging());
            if (!Auditing.IsNull() && !Auditing().IsNull()) resolver.Register(typeof(IAuditProvider), Auditing());
            if (!Cache.IsNull() && !Cache().IsNull()) resolver.Register(typeof(ICache), Cache());
            if (!Localization.IsNull() && !Localization().IsNull()) resolver.Register(typeof(ILocalizationProvider), Localization());
            if (!Storage.IsNull() && !Storage().IsNull()) resolver.Register(typeof(IStorageProvider), Storage());
            if (!MessageChannel.IsNull() && !MessageChannel().IsNull()) resolver.Register(typeof(IMessageChannel), MessageChannel());
            if (!AuthenticationManager.IsNull() && !AuthenticationManager().IsNull()) resolver.Register(typeof(IAuthenticationManager), AuthenticationManager());
            if (!AuthorizationManager.IsNull() && !AuthorizationManager().IsNull()) resolver.Register(typeof(IAuthorizationManager), AuthorizationManager());
            if (!TaskRegistry.IsNull() && !TaskRegistry().IsNull()) resolver.Register(typeof(ITaskRegistry), TaskRegistry());
            if (!PermissionProvider.IsNull() && !PermissionProvider().IsNull()) resolver.Register(typeof(IPermissionProvider), PermissionProvider());
            if (!Benchmarker.IsNull() && !Benchmarker().IsNull()) resolver.Register(typeof(IBenchmarker), Benchmarker());
            if (!PermissionManager.IsNull() && !PermissionManager().IsNull()) resolver.Register(typeof(IPermissionManager), PermissionManager());
            
            resolver.RegisterSingleton<CriticalErrorCallbackManager>();
            resolver.RegisterSingleton<IProcessFactory, ProcessFactory>();
            resolver.RegisterSingleton<IProcessExecutor, ProcessExecutor>();

            resolver.Register<IBusinessProcessContext, BusinessProcessContext>();
            resolver.Register<IApiProcessContext, ApiProcessContext>();
            resolver.Register<IDistributedProcessContext, DistributedProcessContext>();
            resolver.Register<IFileExportProcessContext, FileExportProcessContext>();
            resolver.Register<IFileImportProcessContext, FileImportProcessContext>();
            resolver.Register<IRecurringProcessContext, RecurringProcessContext>();

            resolver.RegisterSingleton<Mediator>();

            RegisterProcesses(config, resolver, scanAssemblies);
            RegisterErrorHendling(config, resolver);
            RegisterJsonSerializerSettings(config, resolver);
            RegisterSmtp(config, resolver);
            RegisterSyncLogProvider(config, resolver);

            var services = SystemBootstrapper.Init(resolver, scanAssemblies);

            return services;
        }

        private void RegisterProcesses(ApplicationConfiguration config, IRegistrationService resolver, params Assembly[] scanAssemblies)
        {
            var processRepo = new ProcessRepository(scanAssemblies);
            resolver.Register(processRepo);
            processRepo.All().ForEach(type => resolver.Register(type));
        }

        private void RegisterErrorHendling(ApplicationConfiguration config, IRegistrationService resolver)
        {
            if (!ErrorHandling.IsNull() && !ErrorHandling().IsNull())
            {
                resolver.Register(typeof(IStrategyBuilder), ErrorHandling());

                var handler = ErrorHandling().Build();

                if (!handler.IsNull())
                {
                    resolver.Register(typeof(IStrategyHandler), handler);
                }
            }
        }

        private void RegisterJsonSerializerSettings(ApplicationConfiguration config, IRegistrationService resolver)
        {
            if (!JsonSerializerSettings.IsNull() && !JsonSerializerSettings().IsNull())
            {
                resolver.Register(typeof(JsonSerializerSettings), JsonSerializerSettings());
                JsonConvert.DefaultSettings = JsonSerializerSettings;
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
            }
        }

        private void RegisterSyncLogProvider(ApplicationConfiguration config, IRegistrationService resolver)
        {
            if (!TaskRegistry.IsNull() && !TaskRegistry().IsNull())
            {
                if (RecurringTaskLogProvider.IsNull() || RecurringTaskLogProvider().IsNull())
                {
                    RecurringTaskLogProvider = () => new RecurringTaskLogProvider();
                }

                resolver.Register(RecurringTaskLogProvider);
            }
        }
    }
}