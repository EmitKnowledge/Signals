using App.Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Signals.Aspects.Caching.Enums;
using Signals.Aspects.Caching.InMemory;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.CommunicationChannels.ServiceBus.Configurations;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI.Autofac;
using Signals.Aspects.ErrorHandling.Polly;
using Signals.Aspects.ErrorHandling.Strategies;
using Signals.Aspects.Localization.File.Configurations;
using Signals.Aspects.Logging.Enums;
using Signals.Aspects.Logging.NLog.Configurations;
using Signals.Core.Background.Configuration;
using Signals.Core.Background.Configuration.Bootstrapping;
using Signals.Core.Common.Instance;
using Signals.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;


namespace App.Client.Background.Service
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<BackgroundServiceService>();
                });

            if (isService)
            {
                builder.RunAsServiceAsync().GetAwaiter().GetResult();
            }
            else
            {
                builder.RunConsoleAsync().GetAwaiter().GetResult();
            }
        }
    }

    /// <summary>
    /// Background service hosting handle
    /// </summary>
    public class BackgroundServiceService : IHostedService, IDisposable
    {
        /// <summary>
        /// IDisposable implementation
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Called when process is starting
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
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

            ApplicationConfiguration.UseProvider(ProviderForFile("application.config.json"));
            BackgroundApplicationConfiguration.UseProvider(ProviderForFile("background.application.config.json"));
            DomainConfiguration.UseProvider(ProviderForFile("domain.config.json"));

            var strategyBuilder = new StrategyBuilder();
            strategyBuilder.Add<Exception>(new RetryStrategy { RetryCount = 3, RetryCooldown = TimeSpan.FromMinutes(5) }).SetAutoHandling(false);

            var config = new BackgroundApplicationBootstrapConfiguration
            {
                RegistrationService = new RegistrationService(),
                CacheConfiguration = new InMemoryCacheConfiguration
                {
                    DataProvider = new InMemoryDataProvider(),
                    ExpirationPolicy = CacheExpirationPolicy.Sliding,
                    ExpirationTime = TimeSpan.FromMinutes(1)
                },
                LoggerConfiguration = new DatabaseLoggingConfiguration
                {
                    Database = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.Database,
                    Host = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.IpAddress,
                    Username = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.Uid,
                    Password = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.Pwd,
                    DataProvider = DataProvider.SqlClient,
                    TableName = "LogEntity"
                },
                LocalizationConfiguration = new JsonDataProviderConfiguration
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
                },
                StrategyBuilder = new StrategyBuilder().SetAutoHandling(false),
                //ChannelConfiguration = new ServiceBusChannelConfiguration
                //{
                //    ConnectionString = DomainConfiguration.Instance.NotificationConfiguration.ConnectionString
                //}
            };

            config.JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
            config.JsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "App.*.dll").Select(file => Assembly.LoadFrom(file)).ToArray();
            config.Bootstrap(assemblies);

            Thread.CurrentThread.CurrentCulture = new CultureInfo(DomainConfiguration.Instance.LocalizationConfiguration.DefaultCulture);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when process is ending
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}