using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Signals.Aspects.Auth.NetCore.Extensions;
using Signals.Aspects.Caching.Enums;
using Signals.Aspects.Caching.InMemory;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.CommunicationChannels.MsSql.Configurations;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI.Autofac;
using Signals.Aspects.Localization.File.Configurations;
using Signals.Aspects.Logging.Enums;
using Signals.Aspects.Logging.NLog.Configurations;
using Signals.Core.Configuration;
using Signals.Core.Web.Configuration;
using Signals.Core.Web.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Signals.Clients.WebApi
{
    public static class SignalsStartup
    {
        /// <summary>
        /// Add signals aspects
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceProvider AddSignals(this IServiceCollection services)
        {
            // Integrate Autofac into Mvc Core
            var registrationService = new RegistrationService();

            var assemblies = Directory.GetFiles(AppContext.BaseDirectory, "*.dll").Select(Assembly.LoadFrom).ToList();

            // Add the app configurations
            AddConfiguration();

            // Authentication
            services.AddAuthentication("Cookies").AddSignalsAuth(opts =>
            {
                opts.ExpireTimeSpan = TimeSpan.FromDays(365);
                opts.Cookie.SameSite = SameSiteMode.None;
                opts.Cookie.Expiration = TimeSpan.FromDays(365);
                opts.Cookie.MaxAge = TimeSpan.FromDays(365);
                opts.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        context.HttpContext.Response.Redirect(WebApplicationConfiguration.Instance.WebUrl);
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\persisted_keys")))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365))
                .SetApplicationName(ApplicationConfiguration.Instance.ApplicationName);

            registrationService.Builder.Populate(services);

            services.AddSignals(config =>
            {
                config.ScanAssemblies = assemblies;
                config.RegistrationService = registrationService;
                config.CacheConfiguration = new InMemoryCacheConfiguration
                {
                    DataProvider = new InMemoryDataProvider(),
                    ExpirationPolicy = CacheExpirationPolicy.Sliding,
                    ExpirationTime = TimeSpan.FromMinutes(1)
                };
                //config.LoggerConfiguration = new DatabaseLoggingConfiguration
                //{
                //    Database = DomainConfiguration.Instance.DatabaseConfiguration.Name,
                //    Host = DomainConfiguration.Instance.DatabaseConfiguration.Host,
                //    Username = DomainConfiguration.Instance.DatabaseConfiguration.User,
                //    Password = DomainConfiguration.Instance.DatabaseConfiguration.Password,
                //    DataProvider = DataProvider.SqlClient,
                //    TableName = "LogEntry"
                //};
                config.LocalizationConfiguration = new JsonDataProviderConfiguration
                {
                    DirectoryPath = AppDomain.CurrentDomain.BaseDirectory,
                    FileExtension = "app",
                    LocalizationSources = new List<LocalizationSource>
                    {
                        new LocalizationSource
                        {
                            Name = "default",
                            SourcePath = "Translations"
                        }
                    }
                };
                //config.ChannelConfiguration = new MsSqlChannelConfiguration
                //{
                //    ConnectionString = DomainConfiguration.Instance.DatabaseConfiguration.Database.ToString(),
                //    DbTableName = DomainConfiguration.Instance.MessageChannelConfiguration.DbTableName,
                //    MessageListeningStrategy = MessageListeningStrategy.None
                //};
                config.JsonSerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
            });

            services.AddDataProtection()
                // This helps surviving a restart: a same app will find back its keys. Just ensure to create the folder.
                .PersistKeysToFileSystem(new DirectoryInfo(
                    Path.Combine(Directory.GetParent(Environment.CurrentDirectory).ToString(), @"persisted_keys")))
                // This helps surviving a site update: each app has its own store, building the site creates a new app
                .SetApplicationName(ApplicationConfiguration.Instance.ApplicationName)
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365));

            // Wrap Autofac container
            return new AutofacServiceProvider(registrationService.ServiceContainer.Container);
        }

        /// <summary>
        /// Loads configurations from files
        /// </summary>
        /// <returns></returns>
        private static void AddConfiguration()
        {
            FileConfigurationProvider ConfigurationProviderForFile(string name) =>
                new FileConfigurationProvider
                {
                    File = name,
                    Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configs"),
                    ReloadOnAccess = false
                };

            // Required by Signals
            ApplicationConfiguration.UseProvider(ConfigurationProviderForFile("app.config.json"));
            WebApplicationConfiguration.UseProvider(ConfigurationProviderForFile("web.config.json"));
        }
    }
}
