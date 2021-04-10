using Signals.Aspects.Logging.Configurations;
using Signals.Aspects.Logging.Enums;

namespace Signals.Aspects.Logging.NLog.Configurations
{
    /// <summary>
    /// Configuration for database logging
    /// </summary>
    public class DatabaseLoggingConfiguration : ILoggingConfiguration
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public DatabaseLoggingConfiguration()
        {
            DataProvider = DataProvider.SqlClient;
            TableName = "LogEntry";
        }

        /// <summary>
        /// Database host address
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Database
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Database username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Database password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Database provider.
        /// Default value: DataProvider.SqlClient
        /// </summary>
        public DataProvider DataProvider { get; set; }

        /// <summary>
        /// Minimum required log level to be logged
        /// </summary>
        public LogLevel MinimumLevel { get; set; }

        /// <summary>
        /// Connection stirng builder
        /// </summary>
        internal string ConnectionString => $"Server={Host};Database={Database};User Id={Username};Password={Password};";
    }
}
