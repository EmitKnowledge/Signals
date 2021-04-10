using App.Domain.Configuration;
using Dapper;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI;
using Signals.Aspects.DI.Autofac;
using Signals.Core.Common.Instance;
using Signals.Core.Configuration;
using Signals.Core.Configuration.Bootstrapping;
using Signals.Core.Processes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using Signals.Aspects.Caching.Enums;
using Signals.Aspects.Caching.InMemory;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.Localization.File.Configurations;
using Signals.Aspects.Logging.Enums;
using Signals.Aspects.Logging.NLog.Configurations;
using Signals.Aspects.ErrorHandling.Polly;

namespace App.Test.Integration
{
    public class TestBootstrapConfiguraiton : ApplicationBootstrapConfiguration
    {
    }

    public class BaseProcessesTest
    {
        /// <summary>
        /// Executed when test class is initialized
        /// </summary>
        public BaseProcessesTest()
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
            DomainConfiguration.UseProvider(ProviderForFile("domain.config.json"));

            TestBootstrapConfiguraiton config = new TestBootstrapConfiguraiton
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
                StrategyBuilder = new StrategyBuilder().SetAutoHandling(false)
            };

            config.JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
            config.JsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            var assemblies = Directory.GetFiles(AppContext.BaseDirectory, "*.dll").Select(file => Assembly.LoadFrom(file)).ToArray();

            config.Bootstrap(assemblies);

            SystemBootstrapper.Bootstrap(this);

        }

        private static void CleanupDatabase()
        {
            var scsb = new SqlConnectionStringBuilder(DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.ConnectionString);
            scsb.MultipleActiveResultSets = true;
            var cs = scsb.ConnectionString;
            using (var connection = new SqlConnection(cs))
            {
                connection.Open();

                // truncata token tables
                //connection.Execute("truncate table [User]; DBCC CHECKIDENT ('[User]', RESEED, 0);");

                connection.Close();
            }
        }
    }
}