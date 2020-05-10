using App.Domain.Configuration;
using App.Domain.Processes.Generic;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Signals.Aspects.Auth.NetCore.Extensions;
using Signals.Aspects.Caching.Enums;
using Signals.Aspects.Caching.InMemory;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.CommunicationChannels.ServiceBus.Configurations;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI;
using Signals.Aspects.DI.Autofac;
using Signals.Aspects.Localization.File.Configurations;
using Signals.Aspects.Logging.Enums;
using Signals.Aspects.Logging.NLog.Configurations;
using Signals.Core.Common.Instance;
using Signals.Core.Configuration;
using Signals.Core.Configuration.Bootstrapping;
using Signals.Core.Processes;
using Signals.Core.Web.Configuration;
using Signals.Core.Web.Configuration.Bootstrapping;
using Signals.Core.Web.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Signals.Aspects.ErrorHandling.Polly;

namespace App.Client.Web
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
            // integrate autofac into mvc.core
            var registrationService = new RegistrationService();

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

            registrationService.Builder.Populate(services);
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
                    config.LoggerConfiguration = new DatabaseLoggingConfiguration
                    {
                        Database = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.Database,
                        Host = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.IpAddress,
                        Username = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.Uid,
                        Password = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.Pwd,
                        DataProvider = DataProvider.SqlClient,
                        TableName = "LogEntity"
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
                    //config.ChannelConfiguration = new ServiceBusChannelConfiguration
                    //{
                    //    ConnectionString = DomainConfiguration.Instance.NotificationConfiguration.ConnectionString
                    //};
                    config.StrategyBuilder = new StrategyBuilder().SetAutoHandling(false);
                });

            // wrap autofac container
            return new AutofacServiceProvider(registrationService.ServiceContainer.Container);
        }

        /// <summary>
        /// Load configuration from files
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static void AddConfiguration(this IServiceCollection services)
        {
            string environment = null;
            FileConfigurationProvider ProviderForFile(string name) => new FileConfigurationProvider
            {
                File = name,
                Path = environment.IsNullOrEmpty() ? Path.Combine(AppContext.BaseDirectory, $"configs") : Path.Combine(AppContext.BaseDirectory, $"configs", environment),
                ReloadOnAccess = false
            };

            EnvironmentConfiguration.UseProvider(ProviderForFile("environment.config.json"));
            environment = EnvironmentConfiguration.Instance.Environment;

            WebApplicationConfiguration.UseProvider(ProviderForFile("web.application.config.json"));
            ApplicationConfiguration.UseProvider(ProviderForFile("application.config.json"));
            DomainConfiguration.UseProvider(ProviderForFile("domain.config.json"));
        }

        /// <summary>
        /// Configure JSON serialization
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static void ConfigureJsonSerialization(this ApplicationBootstrapConfiguration config)
        {
            config.JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
            config.JsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        }
    }
}