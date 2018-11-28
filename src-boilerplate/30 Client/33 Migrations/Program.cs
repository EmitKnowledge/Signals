using App.Client.Migrations.Base;
using App.Domain.Configuration;
using SimpleMigrations;
using SimpleMigrations.Console;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace App.Client.Migrations
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ExecuteMigrations(GetOpenConnection(), args);
        }

        private static void ExecuteMigrations(IDbConnection connection, string[] args)
        {
            if (connection == null) return;
            using (var conn = connection)
            {
                var databaseProvider = new SqlDatabaseProvider(conn);

                var migrator = new SimpleMigrator<IDbConnection, BaseMigration>(Assembly.GetEntryAssembly(), databaseProvider);

                try
                {
                    if (args == null || args.Length == 0)
                    {
                        migrator.Load();
                        migrator.MigrateToLatest();
                        //migrator.MigrateTo(1);
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
}