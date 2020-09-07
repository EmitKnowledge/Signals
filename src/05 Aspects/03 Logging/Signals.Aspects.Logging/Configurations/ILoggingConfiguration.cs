using Signals.Aspects.Logging.Enums;

namespace Signals.Aspects.Logging.Configurations
{
    /// <summary>
    /// Logging configuration
    /// </summary>
    public interface ILoggingConfiguration
    {
        /// <summary>
        /// Minimum required log level to be logged
        /// </summary>
        LogLevel MinimumLevel { get; set; }
    }
}
