using Serilog;
using Serilog.Sinks.MSSqlServer;
using Signals.Aspects.Logging.Enums;
using Signals.Aspects.Logging.Serilog;
using Signals.Aspects.Logging.Serilog.Configurations;
using System;
using System.Data.SqlClient;
using System.IO;
using Signals.Tests.Configuration;
using Xunit;

namespace Signals.Tests.Logging
{
    public class SerilogTests
    {
        private static object @lock = new object();
        private static BaseTestConfiguration _configuration = BaseTestConfiguration.Instance;

        [Fact]
        public void FileLoggerMinLevelWarn_LogsInfo_FileIsEmpty()
        {
            var fileName = $@"{DateTime.Today.ToString("yyyy-MM-dd")}.log";
            var filePath = $@"{Environment.CurrentDirectory}\{fileName}";

            File.Delete(filePath);

            var message = "Some entry";
            Aspects.Logging.ILogger logger = new SerilogLogger(new SerilogLoggingConfiguration
            {
                MinimumLevel = LogLevel.Warn,
                SerilogConfiguration = new LoggerConfiguration().WriteTo.File(filePath, shared: true)
            });

            logger.Info(message);

			var fileText = IoHelper.ReadAllText(filePath);
			Assert.Contains("", fileText);
		}

        [Fact]
        public void FileLogger_LogsInfo_FileExists()
        {
            var fileName = $@"{DateTime.Today.ToString("yyyy-MM-dd")}.log";
            var filePath = $@"{Environment.CurrentDirectory}\{fileName}";

            File.Delete(filePath);

            var message = "Some entry";
            Aspects.Logging.ILogger logger = new SerilogLogger(new SerilogLoggingConfiguration
            {
                MinimumLevel = LogLevel.Trace,
                SerilogConfiguration = new LoggerConfiguration().WriteTo.File(filePath, shared: true)
            });

            logger.Info(message);

            using (var fileReader = new StreamReader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                var fileText = fileReader.ReadToEnd();

                Assert.Contains(message, fileText);
                Assert.Contains("Info", fileText);
            }
        }

        [Fact]
        public void FileLogger_LogsError_FileExists()
        {
            var fileName = $@"{DateTime.Today.ToString("yyyy-MM-dd")}.log";
            var filePath = $@"{Environment.CurrentDirectory}\{fileName}";

            File.Delete(filePath);

            var message = "Some entry";
            Aspects.Logging.ILogger logger = new SerilogLogger(new SerilogLoggingConfiguration
            {
                MinimumLevel = LogLevel.Trace,
                SerilogConfiguration = new LoggerConfiguration().WriteTo.File(filePath, shared: true)
            });

            logger.Error(message);
			var fileText = IoHelper.ReadAllText(filePath);

			Assert.Contains(message, fileText);
			Assert.Contains("Error", fileText);
		}

        [Fact]
        public void DatabaseLogger_LogsInfo_DatabaseLogs()
        {
            var message = "Some entry";
            var TableName = "Log2";

			string connectionString = $"{_configuration.DatabaseConfiguration.ConnectionString};Encrypt=True;TrustServerCertificate=True";

			Aspects.Logging.ILogger logger = new SerilogLogger(new SerilogLoggingConfiguration
            {
                MinimumLevel = LogLevel.Trace,
                SerilogConfiguration = new LoggerConfiguration().WriteTo.MSSqlServer(connectionString, new MSSqlServerSinkOptions()
                {
                    AutoCreateSqlTable = true,
                    TableName = TableName,
                })
            });

            logger.Info(message);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataReader reader = new SqlCommand($@"SELECT * FROM {TableName};", connection).ExecuteReader();
                while (reader.Read())
                {
                    Assert.Contains(message, reader["Message"].ToString());
                }
                reader.Close();

                var command = new SqlCommand($"DELETE FROM {TableName}", connection);
                command.ExecuteNonQuery();
            }
        }

        [Fact]
        public void DatabaseLogger_LogsError_DatabaseLogs()
        {
            var message = "Some entry";
            var TableName = "Log2";
            string connectionString = $"{_configuration.DatabaseConfiguration.ConnectionString};Encrypt=True;TrustServerCertificate=True";

            Aspects.Logging.ILogger logger = new SerilogLogger(new SerilogLoggingConfiguration
            {
                MinimumLevel = LogLevel.Trace,
                SerilogConfiguration = new LoggerConfiguration().WriteTo.MSSqlServer(connectionString, new MSSqlServerSinkOptions()
                {
                    AutoCreateSqlTable = true,
                    TableName = TableName,
                })
            });

            logger.Error(message);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataReader reader = new SqlCommand($@"SELECT * FROM {TableName};", connection).ExecuteReader();
                while (reader.Read())
                {
                    Assert.Contains(message, reader["Message"].ToString());
                }
                reader.Close();

                var command = new SqlCommand($"DELETE FROM {TableName}", connection);
                command.ExecuteNonQuery();
            }
        }
    }
}