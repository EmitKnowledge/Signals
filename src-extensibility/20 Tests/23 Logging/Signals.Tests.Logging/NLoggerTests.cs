using Signals.Aspects.Logging.Configurations;
using Signals.Aspects.Logging.Enums;
using Signals.Aspects.Logging.NLog;
using Signals.Aspects.Logging.NLog.Configurations;
using System;
using System.Data.SqlClient;
using System.IO;
using Xunit;

namespace Signals.Tests.Logging
{
    public class NLoggerTests
    {
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

            logger.Error(new Exception(message));

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
            config.Host = "sql.emitknowledge.com";
            config.Database = "app.db";
            config.Username = "appusr";
            config.Password = "FYGncRXGySXDz6RFNg2e";
            config.DataProvider = DataProvider.SqlClient;
            config.TableName = "Log2";

            Aspects.Logging.ILogger logger = new NLogger(config);

            logger.Info(message);

            string connectionString = $"Data Source={config.Host};Initial Catalog={config.Database}; User Id={config.Username}; Password={config.Password}";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataReader reader = new SqlCommand(@"SELECT * FROM LogEntry;", connection).ExecuteReader();
                while (reader.Read())
                {
                    Assert.Contains(message, reader["Payload"].ToString());
                }
                reader.Close();

                var command = new SqlCommand("DELETE FROM LogEntry", connection);
                command.ExecuteNonQuery();
            }
        }

        [Fact]
        public void DatabaseLogger_LogsError_DatabaseLogs()
        {
            var message = "Some entry";
            var config = new DatabaseLoggingConfiguration();
            config.Host = "sql.emitknowledge.com";
            config.Database = "app.db";
            config.Username = "appusr";
            config.Password = "FYGncRXGySXDz6RFNg2e";
            config.DataProvider = DataProvider.SqlClient;
            config.TableName = "Log2";

            Aspects.Logging.ILogger logger = new NLogger(config);

            logger.Error(new Exception(message));

            string connectionString = $"Data Source={config.Host};Initial Catalog={config.Database}; User Id={config.Username}; Password={config.Password}";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataReader reader = new SqlCommand(@"SELECT * FROM LogEntry;", connection).ExecuteReader();
                while (reader.Read())
                {
                    Assert.Contains(message, reader["Payload"].ToString());
                }
                reader.Close();

                var command = new SqlCommand("DELETE FROM LogEntry", connection);
                command.ExecuteNonQuery();
            }
        }
    }
}