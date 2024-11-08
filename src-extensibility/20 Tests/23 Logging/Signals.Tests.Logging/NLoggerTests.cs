using Signals.Aspects.Logging.Configurations;
using Signals.Aspects.Logging.Enums;
using Signals.Aspects.Logging.NLog;
using Signals.Aspects.Logging.NLog.Configurations;
using System;
using System.Data.SqlClient;
using System.IO;
using Signals.Tests.Configuration;
using Xunit;

namespace Signals.Tests.Logging
{
    public class NLoggerTests
    {
        private static BaseTestConfiguration _configuration = BaseTestConfiguration.Instance;
        
        [Fact]
        public void FileLoggerFromConfig_LogsInfo_FileExists()
        {
            var message = "Some entry";
            Aspects.Logging.ILogger logger = new NLogger(Path.Combine(Environment.CurrentDirectory, "NLog.config"));

            logger.Info(message);

            var fileName = $@"{DateTime.Today.ToString("yyyy-MM-dd")}.log";
            var filePath = $@"{Environment.CurrentDirectory}\{fileName}";

            Assert.True(File.Exists(filePath));

            var fileText = File.ReadAllText(filePath);

            Assert.Contains(message, fileText);
            Assert.Contains("Info", fileText);

            File.Delete(filePath);
        }

        [Fact]
        public void FileLoggerMinLevelWarn_LogsInfo_FileDoesntExist()
        {
            var message = "Some entry";
            var config = new FileLoggingConfiguration();
            config.MinimumLevel = LogLevel.Warn;

            Aspects.Logging.ILogger logger = new NLogger(config);

            logger.Info(message);

            var fileName = $@"{DateTime.Today.ToString("yyyy-MM-dd")}.log";
            var filePath = $@"{Environment.CurrentDirectory}\{fileName}";

            Assert.False(File.Exists(filePath));
        }

        [Fact]
        public void FileLogger_LogsInfo_FileExists()
        {
            var message = "Some entry";
            var config = new FileLoggingConfiguration();
            Aspects.Logging.ILogger logger = new NLogger(config);

            logger.Info(message);

            var fileName = $@"{DateTime.Today.ToString("yyyy-MM-dd")}.log";
            var filePath = $@"{Environment.CurrentDirectory}\{fileName}";

            Assert.True(File.Exists(filePath));

            var fileText = File.ReadAllText(filePath);

            Assert.Contains(message, fileText);
            Assert.Contains("Info", fileText);

            File.Delete(filePath);
        }

        [Fact]
        public void FileLogger_LogsError_FileExists()
        {
            var message = "Some entry";
            var config = new FileLoggingConfiguration();
            Aspects.Logging.ILogger logger = new NLogger(config);

            logger.Error(message);

            var fileName = $@"{DateTime.Today.ToString("yyyy-MM-dd")}.log";
            var filePath = $@"{Environment.CurrentDirectory}\{fileName}";

            Assert.True(File.Exists(filePath));

            var fileText = File.ReadAllText(filePath);

            Assert.Contains(message, fileText);
            Assert.Contains("Error", fileText);

            File.Delete(filePath);
        }

        [Fact]
        public void DatabaseLogger_LogsInfo_DatabaseLogs()
        {
            var message = "Some entry";
            var config = new DatabaseLoggingConfiguration();
            config.Host = _configuration.DatabaseConfiguration.Server;
            config.Database = _configuration.DatabaseConfiguration.Database;
            config.Username = _configuration.DatabaseConfiguration.UserName;
            config.Password = _configuration.DatabaseConfiguration.Password;
            config.DataProvider = DataProvider.SqlClient;
            config.TableName = "Log2";

            Aspects.Logging.ILogger logger = new NLogger(config);

            logger.Info(message);

            string connectionString = _configuration.DatabaseConfiguration.ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataReader reader = new SqlCommand($@"SELECT * FROM {config.TableName};", connection).ExecuteReader();
                while (reader.Read())
                {
                    Assert.Contains(message, reader["Payload"].ToString());
                }
                reader.Close();

                var command = new SqlCommand($"DELETE FROM {config.TableName}", connection);
                command.ExecuteNonQuery();
            }
        }

        [Fact]
        public void DatabaseLogger_LogsError_DatabaseLogs()
        {
            var message = "Some entry";
            var config = new DatabaseLoggingConfiguration();
            config.Host = _configuration.DatabaseConfiguration.Server;
            config.Database = _configuration.DatabaseConfiguration.Database;
            config.Username = _configuration.DatabaseConfiguration.UserName;
            config.Password = _configuration.DatabaseConfiguration.Password;
            config.DataProvider = DataProvider.SqlClient;
            config.TableName = "Log2";

            Aspects.Logging.ILogger logger = new NLogger(config);

            logger.Info(message);

            string connectionString = _configuration.DatabaseConfiguration.ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataReader reader = new SqlCommand($@"SELECT * FROM {config.TableName};", connection).ExecuteReader();
                while (reader.Read())
                {
                    Assert.Contains(message, reader["Payload"].ToString());
                }
                reader.Close();

                var command = new SqlCommand($"DELETE FROM {config.TableName}", connection);
                command.ExecuteNonQuery();
            }
        }
    }
}