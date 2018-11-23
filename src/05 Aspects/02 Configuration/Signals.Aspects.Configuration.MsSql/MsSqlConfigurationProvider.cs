﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Signals.Aspects.Configuration.MsSql
{
    public class MsSqlConfigurationProvider : IConfigurationProvider
    {
        /// <summary>
        /// The connection string to the database where the configuration is loaded from
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The table name where the configuration is loaded from
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// The column that stores the configuration key
        /// </summary>
        public string KeyColumnName { get; set; }

        /// <summary>
        /// The column that stores the configuration itself
        /// </summary>
        public string ValueColumnName { get; set; }

        /// <summary>
        /// Indicates if the configuration should be reloaded whenever it's accessed
        /// </summary>
        public bool ReloadOnAccess { get; set; }

        /// <summary>
        /// Loads the configuration from mssql database
        /// </summary>
        public BaseConfiguration<T> Load<T>(string key) where T : BaseConfiguration<T>, new()
        {

            // Check if the key has been set
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("The configuration key must not be null or empty.");
            }

            // Load the configuration from mssql database
            using (var connection = new SqlConnection(ConnectionString))
            {
                // Open the connection
                connection.Open();

                // If connection string is valid, then proceed with checking if the specified provider is supported in the database
                EnsureDatabaseIsSet<T>(key);

                var query =
                    $@"
                        SELECT [{ValueColumnName}]
                        FROM [{TableName}] 
                        WHERE [{KeyColumnName}] = @key
                    ";

                var command = new SqlCommand(query, connection);
	            // set the filter value
	            command.Parameters.Add("key", SqlDbType.NVarChar);
	            command.Parameters["key"].Value = key;

				var content = command.ExecuteScalar()?.ToString();

                if (content == null)
                {
                    throw new Exception("No configuration is set in the database for the configuration key.");
                }

                // Deserialize the json config
                var instance = JsonConvert.DeserializeObject<T>(content);

                // Validate if the configuration meets the validation rules
                var valContext = new ValidationContext(instance);
                var valResults = new List<ValidationResult>();

                if (Validator.TryValidateObject(instance, valContext, valResults, true)) return instance;
	            throw new Exception(string.Join(Environment.NewLine, valResults.Select(x => x.ErrorMessage)));
			}
        }

        /// <summary>
        /// Reloads the configuration
        /// </summary>
        public BaseConfiguration<T> Reload<T>(string key) where T : BaseConfiguration<T>, new()
        {
            if (ReloadOnAccess)
                return Load<T>(key);

            return null;
        }

        /// <summary>
        /// Check if the data table exists
        /// </summary>
        private void EnsureDatabaseIsSet<T>(string key) where T : BaseConfiguration<T>, new()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query =
                    $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        JOIN sys.schemas s 
	                        ON (t.schema_id = s.schema_id) 
	                        WHERE s.name = 'dbo' AND t.name = '{TableName}'
                        ) 
                        CREATE TABLE dbo.[{TableName}]
                        (
                            [Id] INT IDENTITY(1,1) NOT NULL, 
                            [{KeyColumnName}] NVARCHAR(MAX) NOT NULL, 
                            [{ValueColumnName}] NVARCHAR(MAX)
                        )
                    ";

                var command = new SqlCommand(query, connection);
                connection.Open();
                command.ExecuteNonQuery();

                var serializedObject = JsonConvert.SerializeObject(new T());

                var initialConfigurationQuery =
                    $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM dbo.[{TableName}] tbl
	                        WHERE tbl.[{KeyColumnName}] = '{key}'
                        ) 
                        INSERT INTO dbo.[{TableName}]
                        (
                            [{KeyColumnName}],
                            [{ValueColumnName}]
                        )
                        VALUES
                        (
                            '{key}',
                            '{serializedObject}'
                        )
                    ";

                command = new SqlCommand(initialConfigurationQuery, connection);
                command.ExecuteNonQuery();

                connection.Close();
            }
        }
    }

    public class DefaultMsSqlConfigurationProvider : MsSqlConfigurationProvider
    {
        /// <summary>
        /// Creates default MSSQL Configuration provider
        /// </summary>
        /// <param name="connectionString"></param>
        public DefaultMsSqlConfigurationProvider(string connectionString)
        {
            ConnectionString = connectionString;
            TableName = "Configuration";
            KeyColumnName = "Key";
            ValueColumnName = "Value";
        }
    }
}