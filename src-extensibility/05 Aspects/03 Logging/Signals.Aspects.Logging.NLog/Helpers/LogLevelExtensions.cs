using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Aspects.Logging.NLog.Helpers
{
    internal static class LogLevelExtensions
    {
        public static LogLevel AsNLog(this Enums.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case Enums.LogLevel.Trace:
                    return LogLevel.Trace;
                case Enums.LogLevel.Debug:
                    return LogLevel.Debug;
                case Enums.LogLevel.Info:
                    return LogLevel.Info;
                case Enums.LogLevel.Warn:
                    return LogLevel.Warn;
                case Enums.LogLevel.Error:
                    return LogLevel.Error;
                case Enums.LogLevel.Fatal:
                    return LogLevel.Fatal;
                default:
                    return LogLevel.Trace;
            }
        }

        public static Enums.LogLevel AsInternal(this LogLevel logLevel)
        {
            switch (logLevel.Name)
            {
                case nameof(LogLevel.Trace):
                    return Enums.LogLevel.Trace;
                case nameof(LogLevel.Debug):
                    return Enums.LogLevel.Debug;
                case nameof(LogLevel.Info):
                    return Enums.LogLevel.Info;
                case nameof(LogLevel.Warn):
                    return Enums.LogLevel.Warn;
                case nameof(LogLevel.Error):
                    return Enums.LogLevel.Error;
                case nameof(LogLevel.Fatal):
                    return Enums.LogLevel.Fatal;
                default:
                    return Enums.LogLevel.Trace;
            }
        }
    }
}
