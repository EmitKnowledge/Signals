using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Signals.Aspects.Configuration.MsSql
{
    /// <summary>
    /// Configuration provider for MSSql
    /// </summary>
    public class MsSqlConfigurationProvider : IConfigurationProvider
    {
        private bool isDirty;

        /// <summary>
        /// Default CTOR with ConnectionString
        /// </summary>
        public MsSqlConfigurationProvider(string connectionString)
        {
            isDirty = true;
            ConnectionString = connectionString;
            TableName = "Configuration";
            KeyColumnName = "Key";
            ValueColumnName = "Value";

            EnsureDatabaseTable();
        }

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
                EnsureDatabaseKey<T>(key);

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

                if (Validator.TryValidateObject(instance, valContext, valResults, true))
                {
                    isDirty = false;
                    return instance;
                }
                throw new Exception(string.Join(Environment.NewLine, valResults.Select(x => x.ErrorMessage)));
            }
        }

        /// <summary>
        /// Reloads the configuration
        /// </summary>
        public BaseConfiguration<T> Reload<T>(string key) where T : BaseConfiguration<T>, new()
        {
            if (ReloadOnAccess || isDirty)
                return Load<T>(key);

            return null;
        }

        /// <summary>
        /// Check if the data table exists
        /// </summary>
        private void EnsureDatabaseTable()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query =
                    $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        WHERE t.name = '{TableName}'
                        ) 
                        CREATE TABLE [{TableName}]
                        (
                            [Id] INT IDENTITY(1,1) NOT NULL, 
                            [{KeyColumnName}] NVARCHAR(MAX) NOT NULL, 
                            [{ValueColumnName}] NVARCHAR(MAX)
                        )
                    ";

                var command = new SqlCommand(query, connection);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// Check if the data row exists
        /// </summary>
        private void EnsureDatabaseKey<T>(string key) where T : BaseConfiguration<T>, new()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var serializedObject = JsonConvert.SerializeObject(new T());
                var initialConfigurationQuery =
                    $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM [{TableName}] tbl
	                        WHERE tbl.[{KeyColumnName}] = '{key}'
                        ) 
                        INSERT INTO [{TableName}]
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

                var command = new SqlCommand(initialConfigurationQuery, connection);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// Update the configuration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        public void Update<T>(T configuration) where T : BaseConfiguration<T>, new()
        {
            // Check if the key has been set
            if (string.IsNullOrEmpty(configuration.Key))
            {
                throw new ArgumentException("The configuration key must not be null or empty.");
            }

            // Load the configuration from mssql database
            using (var connection = new SqlConnection(ConnectionString))
            {
                // Open the connection
                connection.Open();

                // If connection string is valid, then proceed with checking if the specified provider is supported in the database
                EnsureDatabaseKey<T>(configuration.Key);

                // Deserialize the json config
                var content = JsonConvert.SerializeObject(configuration);

                var query =
                    $@"
                        UPDATE [{TableName}] 
                        SET [{ValueColumnName}] =  @content
                        WHERE [{KeyColumnName}] = @key
                    ";

                var command = new SqlCommand(query, connection);
                // set the query values
                command.Parameters.Add("key", SqlDbType.NVarChar);
                command.Parameters.Add("content", SqlDbType.NVarChar);
                command.Parameters["key"].Value = configuration.Key;
                command.Parameters["content"].Value = content;

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new Exception("No configuration is set in the database for the configuration key.");
                }

                isDirty = true;
            }
        }
    }
}