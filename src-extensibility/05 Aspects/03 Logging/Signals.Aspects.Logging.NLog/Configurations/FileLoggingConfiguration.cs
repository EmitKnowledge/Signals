using Signals.Aspects.Logging.Configurations;
using Signals.Aspects.Logging.Enums;

namespace Signals.Aspects.Logging.NLog.Configurations
{
    /// <summary>
    /// Configuration for file logging
    /// </summary>
    public class FileLoggingConfiguration : ILoggingConfiguration
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public FileLoggingConfiguration()
        {
            MessageTemplate = "${longdate} ${logger} ${message}${exception:format=ToString}";
            MaxArchiveFiles = 90;
            LogFileDirectory = "${basedir}";
        }

        /// <summary>
        /// Logged message layout template.
        /// Default value: ${longdate} ${logger} ${message}${exception:format=ToString}
        /// </summary>
        public string MessageTemplate { get; set; }

        /// <summary>
        /// Rolling document maximum files history.
        /// Default value: 90
        /// </summary>
        public int MaxArchiveFiles { get; set; }

        /// <summary>
        /// Base directory for log files.
        /// Default value: ${basedir}
        /// </summary>
        public string LogFileDirectory { get; set; }

        /// <summary>
        /// Minimum required log level to be logged
        /// </summary>
        public LogLevel MinimumLevel { get; set; }
    }
}