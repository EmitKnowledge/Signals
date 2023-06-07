using Serilog.Events;

namespace Signals.Aspects.Logging.Serilog.Helpers
{
    internal static class LogLevelExtensions
    {
        public static LogEventLevel AsSerilog(this Enums.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case Enums.LogLevel.Trace:
                    return LogEventLevel.Verbose;
                case Enums.LogLevel.Debug:
                    return LogEventLevel.Debug;
                case Enums.LogLevel.Info:
                    return LogEventLevel.Information;
                case Enums.LogLevel.Warn:
                    return LogEventLevel.Warning;
                case Enums.LogLevel.Error:
                    return LogEventLevel.Error;
                case Enums.LogLevel.Fatal:
                    return LogEventLevel.Fatal;
                default:
                    return LogEventLevel.Verbose;
            }
        }

        public static Enums.LogLevel AsInternal(this LogEventLevel logLevel)
        {
            switch (logLevel)
            {
                case LogEventLevel.Verbose:
                    return Enums.LogLevel.Trace;
                case LogEventLevel.Debug:
                    return Enums.LogLevel.Debug;
                case LogEventLevel.Information:
                    return Enums.LogLevel.Info;
                case LogEventLevel.Warning:
                    return Enums.LogLevel.Warn;
                case LogEventLevel.Error:
                    return Enums.LogLevel.Error;
                case LogEventLevel.Fatal:
                    return Enums.LogLevel.Fatal;
                default:
                    return Enums.LogLevel.Trace;
            }
        }
    }
}
