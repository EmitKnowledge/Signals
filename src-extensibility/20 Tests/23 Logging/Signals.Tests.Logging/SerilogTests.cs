using Serilog;
using Serilog.Sinks.File;
using Serilog.Sinks.MSSqlServer;
using Signals.Aspects.Logging.Enums;
using Signals.Aspects.Logging.Serilog;
using Signals.Aspects.Logging.Serilog.Configurations;
using System;
using System.Data.SqlClient;
using System.IO;
using Xunit;

namespace Signals.Tests.Logging
{
    public class SerilogTests
    {
        private static object @lock = new object();

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

            using (var fileReader = new StreamReader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                var fileText = fileReader.ReadToEnd();

                Assert.Contains("", fileText);
            }
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

            logger.Error(new Exception(message));

            using (var fileReader = new StreamReader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                var fileText = fileReader.ReadToEnd();

                Assert.Contains(message, fileText);
                Assert.Contains("Error", fileText);
            }
        }

        [Fact]
        public void DatabaseLogger_LogsInfo_DatabaseLogs()
        {
            var message = "Some entry";
            var Host = "sql.emitknowledge.com";
            var Database = "app.db";
            var Username = "appusr";
            var Password = "FYGncRXGySXDz6RFNg2e";
            var TableName = "Log2";
            string connectionString = $"Data Source={Host};Initial Catalog={Database}; User Id={Username}; Password={Password}";

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
            var Host = "sql.emitknowledge.com";
            var Database = "app.db";
            var Username = "appusr";
            var Password = "FYGncRXGySXDz6RFNg2e";
            var TableName = "Log2";
            string connectionString = $"Data Source={Host};Initial Catalog={Database}; User Id={Username}; Password={Password}";

            Aspects.Logging.ILogger logger = new SerilogLogger(new SerilogLoggingConfiguration
            {
                MinimumLevel = LogLevel.Trace,
                SerilogConfiguration = new LoggerConfiguration().WriteTo.MSSqlServer(connectionString, new MSSqlServerSinkOptions()
                {
                    AutoCreateSqlTable = true,
                    TableName = TableName,
                })
            });

            logger.Error(new Exception(message));

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