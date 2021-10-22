using Serilog;
using Signals.Aspects.Logging.Configurations;
using Signals.Aspects.Logging.Enums;

namespace Signals.Aspects.Logging.Serilog.Configurations
{
    /// <summary>
    /// Configuration based on serilog config
    /// </summary>
    public class SerilogLoggingConfiguration : ILoggingConfiguration
    {
        /// <summary>
        /// Serilog configuration
        /// </summary>
        public LoggerConfiguration SerilogConfiguration { get; set; }

        /// <summary>
        /// Minimum required log level to be logged
        /// </summary>
        public LogLevel MinimumLevel { get; set; }
    }
}
