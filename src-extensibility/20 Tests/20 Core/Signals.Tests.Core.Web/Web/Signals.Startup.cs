using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Signals.Aspects.Auditing.AuditNET.Configurations;
using Signals.Aspects.Auth.NetCore.Extensions;
using Signals.Aspects.Benchmarking.Database.Configurations;
using Signals.Aspects.Caching.Enums;
using Signals.Aspects.Caching.InMemory;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.CommunicationChannels.MsSql.Configurations;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI;
using Signals.Aspects.ErrorHandling.Polly;
using Signals.Aspects.Localization.File.Configurations;
using Signals.Aspects.Logging.Enums;
using Signals.Aspects.Logging.NLog.Configurations;
using Signals.Aspects.Storage.File.Configurations;
using Signals.Core.Configuration;
using Signals.Core.Configuration.Bootstrapping;
using Signals.Core.Processing.Behaviour;
using Signals.Core.Web.Configuration;
using Signals.Core.Web.Extensions;
using Signals.Tests.Core.Web.Web.config;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Signals.Tests.Core.Web.Web
{
    /// <summary>
    /// Signals bootstrapping class
    /// </summary>
    public static class SignalsStartup
    {

        /// <summary>
        /// Add signals aspects
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceProvider AddSignals(this IServiceCollection services)
        {
            IRegistrationService registrationService = null;
            if (Startup.ContainerName.ToLower() == "autofac")
                registrationService = new Aspects.DI.Autofac.RegistrationService();
            else if (Startup.ContainerName.ToLower() == "simpleinjector")
                registrationService = new Aspects.DI.SimpleInjector.RegistrationService(false);
            else if (Startup.ContainerName.ToLower() == "dotnetcore")
                registrationService = new Aspects.DI.DotNetCore.RegistrationService(services);

            var assemblies = Directory.GetFiles(AppContext.BaseDirectory, "*.dll").Select(Assembly.LoadFrom).ToList();

            services.AddConfiguration();

            services.AddAuthentication("Cookies")
                    .AddSignalsAuth(opts =>
                    {
                        opts.SlidingExpiration = true;
                        opts.ExpireTimeSpan = TimeSpan.FromDays(365);
                        opts.Cookie.Expiration = TimeSpan.FromDays(365);
                        opts.Cookie.MaxAge = TimeSpan.FromDays(365);
                        opts.Cookie.IsEssential = true;
                    });

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, @"..\persisted_keys")))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365))
                .SetApplicationName(ApplicationConfiguration.Instance.ApplicationName);

            if (Startup.ContainerName.ToLower() == "autofac")
                (registrationService as Aspects.DI.Autofac.RegistrationService).Builder.Populate(services);
            else if (Startup.ContainerName.ToLower() == "simpleinjector")
            {
                services.AddSimpleInjector((registrationService as Aspects.DI.SimpleInjector.RegistrationService).Builder, options =>
                {
                    options.AddAspNetCore();
                });
            }
            services
                .AddSignals(config =>
                {
                    config.ConfigureJsonSerialization();
                    config.ScanAssemblies = assemblies;
                    config.RegistrationService = registrationService;
                    config.CacheConfiguration = new InMemoryCacheConfiguration
                    {
                        DataProvider = new InMemoryDataProvider(),
                        ExpirationPolicy = CacheExpirationPolicy.Sliding,
                        ExpirationTime = TimeSpan.FromMinutes(1)
                    };
                    config.StorageConfiguration = new FileStorageConfiguration
                    {
                        Encrypt = false,
                        RootPath = @"C:\StorageTest"
                    };
                    config.AuditingConfiguration = new DatabaseAuditingConfiguration
                    {
                        ConnectionString = DatabaseConfiguration.Instance.ActiveConfiguration.ConnectionString
                    };
                    config.LoggerConfiguration = new DatabaseLoggingConfiguration
                    {
                        Database = DatabaseConfiguration.Instance.ActiveConfiguration.Database,
                        Host = DatabaseConfiguration.Instance.ActiveConfiguration.IpAddress,
                        Username = DatabaseConfiguration.Instance.ActiveConfiguration.Uid,
                        Password = DatabaseConfiguration.Instance.ActiveConfiguration.Pwd,
                        DataProvider = DataProvider.SqlClient,
                        TableName = "LogEntry",
                        MinimumLevel = LogLevel.Trace
                    };
                    config.LocalizationConfiguration = new JsonDataProviderConfiguration
                    {
                        DirectoryPath = Path.Combine(AppContext.BaseDirectory, "system.resources"),
                        FileExtension = "app",
                        LocalizationSources = new List<LocalizationSource>
                        {
                            new LocalizationSource
                            {
                                Name = "Mail messages",
                                SourcePath = "mailmessages"
                            },
                            new LocalizationSource
                            {
                                Name = "Validation rules",
                                SourcePath = "validationrules"
                            },
                            new LocalizationSource
                            {
                                Name = "Pages",
                                SourcePath = "pages"
                            },
                            new LocalizationSource
                            {
                                Name = "Processes",
                                SourcePath = "processes"
                            }
                        }
                    };
                    config.ChannelConfiguration = new MsSqlChannelConfiguration
                    {
                        ConnectionString = DatabaseConfiguration.Instance.ActiveConfiguration.ConnectionString,
                        DbTableName = "EventMessage",
                        MessageListeningStrategy = MessageListeningStrategy.Broker
                    };
                    config.StrategyBuilder = new StrategyBuilder().SetAutoHandling(false);
                    config.BenchmarkingConfiguration = new DatabaseBenchmarkingConfiguration
                    {
                        ConnectionString = DatabaseConfiguration.Instance.ActiveConfiguration.ConnectionString,
                        IsEnabled = true
                    };
                });

            // print error on desktop
            var callbackManager = SystemBootstrapper.GetInstance<CriticalErrorCallbackManager>();
            callbackManager?.OnError((process, type, args, ex) =>
            {
                var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var logFile = Path.Combine(desktop, "STEEMIT_ERRORS.log");

                using (var stream = new StreamWriter(new FileStream(logFile, FileMode.Append, FileAccess.Write)))
                {
                    stream.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {process.Name} failed with exception: {ex.Message} and stack {Environment.NewLine} {ex.StackTrace}");
                }
            });

            if (Startup.ContainerName.ToLower() == "autofac")
                return new AutofacServiceProvider((registrationService as Aspects.DI.Autofac.RegistrationService).ServiceContainer.Container);
            else if (Startup.ContainerName.ToLower() == "dotnetcore")
                return (registrationService as Aspects.DI.DotNetCore.RegistrationService).ServiceContainer.Container;
            else if (Startup.ContainerName.ToLower() == "simpleinjector")
            {
                Startup.Container = (registrationService as Aspects.DI.SimpleInjector.RegistrationService).ServiceContainer.Container;
            }

                return services.BuildServiceProvider();
        }

        /// <summary>
        /// Load configuration from files
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static void AddConfiguration(this IServiceCollection services)
        {
            FileConfigurationProvider ProviderForFile(string name) => new FileConfigurationProvider
            {
                File = name,
                Path = Path.Combine(AppContext.BaseDirectory, $"Web/config"),
                ReloadOnAccess = false
            };

            WebApplicationConfiguration.UseProvider(ProviderForFile("webapp.json"));
            ApplicationConfiguration.UseProvider(ProviderForFile("app.json"));
            DatabaseConfiguration.UseProvider(ProviderForFile("database.json"));
        }

        /// <summary>
        /// Configure JSON serialization
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static void ConfigureJsonSerialization(this ApplicationBootstrapConfiguration config)
        {
            config.JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
        }
    }
}