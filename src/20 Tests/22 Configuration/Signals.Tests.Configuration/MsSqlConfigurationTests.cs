using System;
using System.Data.SqlClient;
using Signals.Aspects.Configuration.MsSql;
using Signals.Tests.Configuration.CustomConfigurations.MsSql.Controllers;
using Xunit;

namespace Signals.Tests.Configuration
{
    public class MsSqlConfigurationTests
    {
        [Fact]
        public void Load_Configuration_From_NonExisting_Db_Should_Return_Valid_Object()
        {
            var connString = "Server=sql.emitknowledge.com;Database=app.db;User Id=appusr;Password=FYGncRXGySXDz6RFNg2e;";
            var configuration = new DefaultMsSqlConfigurationProvider(connString);

            // Delete the configuration table from the database if exists
            using (var connection = new SqlConnection(connString))
            {
                var query =
                    $@"
                        IF EXISTS 
                            (	
                                SELECT * 
	                            FROM sys.tables t 
	                            JOIN sys.schemas s 
	                            ON (t.schema_id = s.schema_id) 
	                            WHERE s.name = 'dbo' AND t.name = '{configuration.TableName}'
                            )
                        DROP TABLE [{configuration.TableName}]
                    ";

                var command = new SqlCommand(query, connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            // Sets up and loads controller configuration using the default MSSQL configuration provider
            ControllersConfiguration.UseProvider(new DefaultMsSqlConfigurationProvider(connString));

            Assert.NotNull(ControllersConfiguration.Instance);
        }

        [Fact]
        public void Load_Configuration_From_Existing_Db_Should_Return_Valid_Object()
        {
            var connString = "Server=sql.emitknowledge.com;Database=app.db;User Id=appusr;Password=FYGncRXGySXDz6RFNg2e;";
            var configuration = new DefaultMsSqlConfigurationProvider(connString);
            
            // Sets up and loads controller configuration using the default MSSQL configuration provider
            ControllersConfiguration.UseProvider(configuration);

            // Make sure the database exists and insert mock configuration json object
            using (var connection = new SqlConnection(connString))
            {
                // Create the database if it does not exist
                var query =
                    $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        JOIN sys.schemas s 
	                        ON (t.schema_id = s.schema_id) 
	                        WHERE s.name = 'dbo' AND t.name = '{configuration.TableName}'
                        ) 
                        CREATE TABLE dbo.[{configuration.TableName}]
                        (
                            [Id] INT IDENTITY(1,1) NOT NULL, 
                            [{configuration.KeyColumnName}] VARCHAR(MAX) NOT NULL, 
                            [{configuration.ValueColumnName}] VARCHAR(MAX)
                        )
                    ";

                connection.Open();

                var command = new SqlCommand(query, connection);
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

                var insertKeyQuery =
                    $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM dbo.[{configuration.TableName}] tbl
	                        WHERE tbl.[{configuration.KeyColumnName}] = '{ControllersConfiguration.Instance.Key}'
                        ) 
                        INSERT INTO dbo.[{configuration.TableName}]
                        (
                            [{configuration.KeyColumnName}],
                            [{configuration.ValueColumnName}]
                        )
                        VALUES
                        (
                            '{ControllersConfiguration.Instance.Key}',
                            NULL
                        )
                    ";

                command = new SqlCommand(insertKeyQuery, connection);
                command.ExecuteNonQuery();

                var insertValueQuery =
                    $@"
                        UPDATE [{configuration.TableName}]
                        SET [{configuration.ValueColumnName}] = '{mockJsonConfiguration}'
                        WHERE [{configuration.KeyColumnName}] = '{ControllersConfiguration.Instance.Key}'
                    ";

                command = new SqlCommand(insertValueQuery, connection);
                command.ExecuteNonQuery();

                connection.Close();
            }

            var appName = ControllersConfiguration.Instance.ApplicationConfiguration.ApplicationName;
            var externalApis = ControllersConfiguration.Instance.ExternalApisConfiguration.ExternalApis;
            var saltLength = ControllersConfiguration.Instance.SecurityConfiguration.SaltLength;

            Assert.NotNull(appName);
            Assert.NotNull(externalApis);
            Assert.Equal("AppName", appName);
            Assert.Single(externalApis);
            Assert.Equal(32, saltLength);
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
