using App.Client.Migrations.Base;
using App.Client.Migrations.Migrations;
using App.Domain.Configuration;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI;
using Signals.Aspects.DI.Autofac;
using Signals.Aspects.ErrorHandling.Polly;
using Signals.Core.Common.Instance;
using Signals.Core.Configuration;
using Signals.Core.Configuration.Bootstrapping;
using Signals.Core.Processes;
using Signals.Core.Processing.Results;
using SimpleMigrations;
using SimpleMigrations.Console;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;

namespace App.Client.Migrations
{
    public class Program
    {
        public static long? RollbackVersionBeforeMigratingToLatest = null;

        public static void Main(string[] args)
        {
            FileConfigurationProvider ProviderForFile(string name) => new FileConfigurationProvider
            {
                File = name,
                Path = Path.Combine(Environment.CurrentDirectory, $"configs"),
                ReloadOnAccess = false
            };

            ApplicationConfiguration.UseProvider(ProviderForFile("application.config.json"));
            DomainConfiguration.UseProvider(ProviderForFile("domain.config.json"));

            MigrationBootstrapConfiguraiton config = new MigrationBootstrapConfiguraiton
            {
                RegistrationService = new RegistrationService(),
            };

            config.JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
            config.JsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            config.StrategyBuilder = new StrategyBuilder().SetAutoHandling(false);

            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "App.*.dll").Select(file => Assembly.LoadFrom(file)).ToArray();

            config.Bootstrap(assemblies);

            ExecuteMigrations(GetOpenConnection(), args);
        }

        private static void ExecuteMigrations(IDbConnection connection, string[] args)
        {
            if (connection == null) return;
            using (var conn = connection)
            {
                var databaseProvider = new SqlDatabaseProvider(conn);

                var migrator = new SimpleMigrator<IDbConnection, BaseMigration>(Assembly.GetCallingAssembly(), databaseProvider);

                try
                {
                    if (args == null || args.Length == 0)
                    {
                        migrator.Load();
                        if (RollbackVersionBeforeMigratingToLatest.HasValue && RollbackVersionBeforeMigratingToLatest > 0)
                            migrator.MigrateTo(RollbackVersionBeforeMigratingToLatest.Value);
                        migrator.MigrateToLatest();
                    }
                    else
                    {
                        var console = new ConsoleRunner(migrator);
                        console.Run(args);
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        private static IDbConnection GetOpenConnection()
        {
            var connectionString = DomainConfiguration.Instance?.DatabaseConfiguration?.ActiveConfiguration?.ConnectionString;
            return BuildConnectionStirng(connectionString);
        }

        private static IDbConnection BuildConnectionStirng(string connectionString)
        {
            if (connectionString == null) return null;
            var scsb = new SqlConnectionStringBuilder(connectionString)
            {
                MultipleActiveResultSets = true
            };
            var cs = scsb.ConnectionString;
            var connection = new SqlConnection(cs);
            return connection;
        }
    }

    public class MigrationBootstrapConfiguraiton : ApplicationBootstrapConfiguration
    {
        public void Bootstrap(params Assembly[] scanAssemblies)
        {
            Resolve(scanAssemblies);
        }

        protected override IServiceContainer Resolve(params Assembly[] scanAssemblies)
        {
            return base.Resolve(scanAssemblies);
        }
    }
}