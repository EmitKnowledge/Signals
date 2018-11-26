using Signals.Aspects.Logging.Configurations;

namespace Signals.Aspects.Logging.NLog.Configurations
{
    /// <summary>
    /// Cofiguration for console logging
    /// </summary>
    public class ConsoleLoggingConfiguration : ILoggingConfiguration
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public ConsoleLoggingConfiguration()
        {
            MessageTemplate = "${longdate} ${logger} ${message}${exception:format=ToString}";
        }

        /// <summary>
        /// Logged message layout template.
        /// Default value: ${longdate} ${logger} ${message}${exception:format=ToString}
        /// </summary>
        public string MessageTemplate { get; set; }
    }
}