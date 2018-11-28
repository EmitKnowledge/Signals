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
using Signals.Core.Processes.Recurring;
using Signals.Core.Processes.Recurring.Logging;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Execution;
using System;
using System.Net;
using System.Net.Mail;
using System.Reflection;

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
        /// Synchronization logging provider
        /// </summary>
        public Func<ISyncLogProvider> SyncLogProvider { get; internal set; }

        /// <summary>
        /// Register all aspects into dependency resolver
        /// </summary>
        /// <returns></returns>
        public IServiceContainer Bootstrap(params Assembly[] scanAssemblies)
        {
            // Proc config validation
            var config = ApplicationConfiguration.Instance;

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
            if (!PermissionManager.IsNull() && !PermissionManager().IsNull()) resolver.Register(typeof(IPermissionManager), PermissionManager());
            if (!ErrorHandling.IsNull() && !ErrorHandling().IsNull())
            {
                resolver.Register(typeof(IStrategyBuilder), ErrorHandling());

                var handler = ErrorHandling().Build();

                if (!handler.IsNull())
                {
                    resolver.Register(typeof(IStrategyHandler), handler);
                }
            }

            resolver.Register<IProcessFactory, ProcessFactory>();
            resolver.Register<IProcessExecutor, ProcessExecutor>();
            resolver.Register<Mediator>();

            var processRepo = new ProcessRepository(scanAssemblies);
            resolver.Register(processRepo);
            processRepo.All().ForEach(type => resolver.Register(type));

            RegisterSmtp(config, resolver);
            RegisterSyncLogProvider(config, resolver);

            var services = SystemBootstrapper.Init(resolver, scanAssemblies);

            return services;
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

                resolver.Register(instance);
            }
        }

        private void RegisterSyncLogProvider(ApplicationConfiguration config, IRegistrationService resolver)
        {
            if (!TaskRegistry.IsNull() && !TaskRegistry().IsNull())
            {
                if(SyncLogProvider.IsNull() || SyncLogProvider().IsNull())
                {
                    SyncLogProvider = () => new SyncLogProvider();
                }

                resolver.Register(SyncLogProvider);
            }
        }
    }
}