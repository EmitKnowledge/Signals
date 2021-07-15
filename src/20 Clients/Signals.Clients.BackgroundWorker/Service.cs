﻿using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Signals.Aspects.BackgroundProcessing.FluentScheduler;
using Signals.Aspects.Caching.Enums;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.CommunicationChannels.ServiceBus.Configurations;
using Signals.Aspects.DI.Autofac;
using Signals.Aspects.Localization.File.Configurations;
using Signals.Core.Background.Configuration;
using Signals.Core.Background.Configuration.Bootstrapping;
using Signals.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using App.Clients.Processes;
using FileConfigurationProvider = Signals.Aspects.Configuration.File.FileConfigurationProvider;

namespace App.Clients.BackgroundWorker
{
    public class Service : IHostedService, IDisposable
    {
        /// <summary>
        /// Called when process is starting
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Setup configuration
            var activeEnvironment = "temp";
            ApplicationConfiguration.UseProvider(new FileConfigurationProvider());
            BackgroundApplicationConfiguration.UseProvider(new FileConfigurationProvider());

            var config = new BackgroundApplicationBootstrapConfiguration
            {
                RegistrationService = new RegistrationService(),
                TaskRegistry = new FluentRegistry(),
                //LoggerConfiguration = new DatabaseLoggingConfiguration
                //{
                //    Database = InfrastructureConfiguration.Instance.Database.Name,
                //    Host = InfrastructureConfiguration.Instance.Database.Server,
                //    Username = InfrastructureConfiguration.Instance.Database.User,
                //    Password = InfrastructureConfiguration.Instance.Database.Password,
                //    DataProvider = DataProvider.SqlClient,
                //    TableName = "LogEntry"
                //},
                CacheConfiguration = new InMemoryCacheConfiguration
                {
                    ExpirationPolicy = CacheExpirationPolicy.Sliding,
                    ExpirationTime = TimeSpan.FromMinutes(1)
                },
                LocalizationConfiguration = new JsonDataProviderConfiguration
                {
                    DirectoryPath = AppDomain.CurrentDomain.BaseDirectory,
                    FileExtension = "json",
                    LocalizationSources = new List<LocalizationSource>
                    {
                        new ()
                        {
                            Name = "default",
                            SourcePath = "Translations"
                        }
                    }
                },
                ChannelConfiguration = new ServiceBusChannelConfiguration
                {
                    ConnectionString = "Endpoint=sb://reipurth-dental.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=qjXNy0Ed7D6E0PY+mfEGxXF4+gJSOXBaVzYD454IDps=",
                    ChannelPrefix = $"{activeEnvironment}_",
                    MaxConcurrentCalls = 10
                },
                JsonSerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            };
            // config.JsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "App.*.dll").Select(Assembly.LoadFrom).ToArray();
            config.Bootstrap(assemblies);
            
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

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        public void Dispose()
        {

        }
    }
}
