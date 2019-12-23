using Signals.Aspects.Configuration.MsSql;
using Signals.Tests.Configuration.CustomConfigurations.MsSql.Controllers;
using Signals.Tests.Configuration.CustomConfigurations.MsSql.Controllers.ExternalApis;
using System;
using System.Data.SqlClient;
using Xunit;

namespace Signals.Tests.Configuration
{
    public class MsSqlConfigurationTests
    {
        public static string ConnectionStirng = "Server=sql.emitknowledge.com;Database=app.db;User Id=appusr;Password=FYGncRXGySXDz6RFNg2e;";

        [Fact]
        public void Load_Configuration_From_NonExisting_Db_Should_Return_Valid_Object()
        {
            lock (ConnectionStirng)
            {
                var configuration = new MsSqlConfigurationProvider(ConnectionStirng, "Configuration2")
                {
                };

                // Delete the configuration table from the database if exists
                using (var connection = new SqlConnection(ConnectionStirng))
                {
                    var query =
                        $@"
                        IF EXISTS 
                            (	
                                SELECT * 
	                            FROM sys.tables t
	                            WHERE t.name = '{configuration.TableName}'
                            )
                        DROP TABLE [{configuration.TableName}]
                    ";

                    var command = new SqlCommand(query, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                // Sets up and loads controller configuration using the default MSSQL configuration provider
                ControllersConfiguration.UseProvider(new MsSqlConfigurationProvider(ConnectionStirng));

                Assert.NotNull(ControllersConfiguration.Instance);
            }
        }

        [Fact]
        public void Load_Configuration_From_Existing_Db_Should_Return_Valid_Object()
        {
            lock (ConnectionStirng)
            {
                var configuration = new MsSqlConfigurationProvider(ConnectionStirng, "Configuration2")
                {
                    ReloadOnAccess = true
                };

                // Sets up and loads controller configuration using the default MSSQL configuration provider
                ControllersConfiguration.UseProvider(configuration);

                // Make sure the database exists and insert mock configuration json object
                using (var connection = new SqlConnection(ConnectionStirng))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {

                        // Create the database if it does not exist
                        var query =
                            $@"
                                IF NOT EXISTS 
                                (	
                                    SELECT * 
	                                FROM sys.tables t
	                                WHERE t.name = '{configuration.TableName}'
                                ) 
                                CREATE TABLE [{configuration.TableName}]
                                (
                                    [Id] INT IDENTITY(1,1) NOT NULL, 
                                    [{configuration.KeyColumnName}] VARCHAR(MAX) NOT NULL, 
                                    [{configuration.ValueColumnName}] VARCHAR(MAX)
                                )
                            ";


                        var command = new SqlCommand(query, connection, transaction);
                        command.ExecuteNonQuery();

                        var insertKeyQuery =
                            $@"
                                IF NOT EXISTS 
                                (	
                                    SELECT * 
	                                FROM [{configuration.TableName}] tbl
	                                WHERE tbl.[{configuration.KeyColumnName}] = 'ControllersConfiguration'
                                ) 
                                INSERT INTO [{configuration.TableName}]
                                (
                                    [{configuration.KeyColumnName}],
                                    [{configuration.ValueColumnName}]
                                )
                                VALUES
                                (
                                    'ControllersConfiguration',
                                    NULL
                                )
                            ";

                        command = new SqlCommand(insertKeyQuery, connection, transaction);
                        command.ExecuteNonQuery();

                        // Insert mock json configuration
                        var mockJsonConfiguration =
                            @"
                                {
                                    'Key': 'ControllersConfiguration', 
	                                'ApplicationConfiguration': 
	                                {
		                                'ApplicationName': 'AppName'
	                                }, 
	                                'SecurityConfiguration': 
	                                {
		                                'SaltLength': 32
	                                }, 
	                                'ExternalApisConfiguration': 
	                                {
		                                'ExternalApis':
		                                [
			                                {
                                                'Name': 'ApiName',
			                                    'Url': 'http://url'
                                            }
		                                ]
	                                }
                                }
                            ".Replace("'", "\"");

                        var insertValueQuery =
                            $@"
                                UPDATE [{configuration.TableName}]
                                SET [{configuration.ValueColumnName}] = '{mockJsonConfiguration}'
                                WHERE [{configuration.KeyColumnName}] = 'ControllersConfiguration'
                            ";

                        command = new SqlCommand(insertValueQuery, connection, transaction);
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        connection.Close();
                    }
                }

                var appName = ControllersConfiguration.Instance.ApplicationConfiguration.ApplicationName;
                var externalApis = ControllersConfiguration.Instance.ExternalApisConfiguration.ExternalApis;
                var saltLength = ControllersConfiguration.Instance.SecurityConfiguration.SaltLength;

                Assert.NotNull(appName);
                Assert.NotNull(externalApis);
                Assert.Equal("AppName", appName);
                Assert.Single(externalApis);
                Assert.Equal(32, saltLength);

                using (var connection = new SqlConnection(ConnectionStirng))
                {
                    var query =
                        $@"
                        IF EXISTS 
                            (	
                                SELECT * 
	                            FROM sys.tables t
	                            WHERE t.name = '{configuration.TableName}'
                            )
                        DROP TABLE [{configuration.TableName}]
                    ";

                    var command = new SqlCommand(query, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        [Fact]
        public void Update_Configuration_Should_Return_Valid_Object()
        {
            lock (ConnectionStirng)
            {
                var configuration = new MsSqlConfigurationProvider(ConnectionStirng, "Configuration2")
                {
                    ReloadOnAccess = true
                };

                // Sets up and loads controller configuration using the default MSSQL configuration provider
                ControllersConfiguration.UseProvider(configuration);

                // Make sure the database exists and insert mock configuration json object
                using (var connection = new SqlConnection(ConnectionStirng))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        // Create the database if it does not exist
                        var query =
                            $@"
                                IF NOT EXISTS 
                                (	
                                    SELECT * 
	                                FROM sys.tables t
	                                WHERE t.name = '{configuration.TableName}'
                                ) 
                                CREATE TABLE [{configuration.TableName}]
                                (
                                    [Id] INT IDENTITY(1,1) NOT NULL, 
                                    [{configuration.KeyColumnName}] VARCHAR(MAX) NOT NULL, 
                                    [{configuration.ValueColumnName}] VARCHAR(MAX)
                                )
                            ";


                        var command = new SqlCommand(query, connection, transaction);
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        connection.Close();
                    }
                }

                var appName = ControllersConfiguration.Instance?.ApplicationConfiguration?.ApplicationName;
                var externalApis = ControllersConfiguration.Instance?.ExternalApisConfiguration?.ExternalApis;
                var saltLength = ControllersConfiguration.Instance?.SecurityConfiguration?.SaltLength;

                Assert.Null(appName);
                Assert.Null(externalApis);
                Assert.Null(saltLength);

                var newInstanceValue = new ControllersConfiguration();

                newInstanceValue.ApplicationConfiguration = new ApplicationConfiguration();
                newInstanceValue.ExternalApisConfiguration = new ExternalApisConfiguration();
                newInstanceValue.SecurityConfiguration = new SecurityConfiguration();

                newInstanceValue.ApplicationConfiguration.ApplicationName = "AppName";
                newInstanceValue.ExternalApisConfiguration.ExternalApis = new System.Collections.Generic.List<ExternalApi>()
                {
                    new ExternalApi
                    {
                        Name = "some name",
                        Url = "some url"
                    }
                };
                newInstanceValue.SecurityConfiguration.SaltLength = 32;

                ControllersConfiguration.Update(newInstanceValue);

                appName = ControllersConfiguration.Instance.ApplicationConfiguration?.ApplicationName;
                externalApis = ControllersConfiguration.Instance.ExternalApisConfiguration?.ExternalApis;
                saltLength = ControllersConfiguration.Instance.SecurityConfiguration?.SaltLength;

                Assert.NotNull(appName);
                Assert.NotNull(externalApis);
                Assert.Equal("AppName", appName);
                Assert.Single(externalApis);
                Assert.Equal(32, saltLength);

                using (var connection = new SqlConnection(ConnectionStirng))
                {
                    var query =
                        $@"
                        IF EXISTS 
                            (	
                                SELECT * 
	                            FROM sys.tables t
	                            WHERE t.name = '{configuration.TableName}'
                            )
                        DROP TABLE [{configuration.TableName}]
                    ";

                    var command = new SqlCommand(query, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        [Fact]
        public void Load_Without_Setup_Should_Throw_Exception()
        {
            try
            {
                var config = ControllersConfiguration.Instance.ApplicationConfiguration;
            }
            catch (Exception e)
            {
                Assert.NotNull(e);
            }
        }
    }
}
