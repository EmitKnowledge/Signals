using Serilog.Core;
using Signals.Aspects.Logging.Enums;
using Signals.Aspects.Logging.Serilog.Configurations;
using Signals.Aspects.Logging.Serilog.Helpers;
using System;

namespace Signals.Aspects.Logging.Serilog
{
    public class SerilogLogger : ILogger
    {
        private SerilogLoggingConfiguration LoggingConfiguration { get; set; }
        private Logger Logger { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="loggingConfiguration"></param>
        public SerilogLogger(SerilogLoggingConfiguration loggingConfiguration)
        {
            this.LoggingConfiguration = loggingConfiguration;
            this.Logger = LoggingConfiguration.SerilogConfiguration.CreateLogger();
        }

        /// <summary>
        /// Log an message consisting of args with log level trace
        /// </summary>
        /// <param name="logEntry"></param>
        public void Trace(LogEntry logEntry)
        {
            if (LoggingConfiguration == null || LoggingConfiguration.MinimumLevel <= LogLevel.Trace)
                InternalLog(logEntry, LogLevel.Trace);
        }

        /// <summary>
        /// Log an message consisting of args with log level trace
        /// </summary>
        /// <param name="message"></param>
        public void Trace(string message)
        {
	        if (LoggingConfiguration == null || LoggingConfiguration.MinimumLevel <= LogLevel.Trace)
		        InternalLog(new LogEntry { Message = message }, LogLevel.Trace);
        }

        /// <summary>
        /// Log an message consisting of args with log level debug
        /// </summary>
        /// <param name="logEntry"></param>
        public void Debug(LogEntry logEntry)
        {
            if (LoggingConfiguration == null || LoggingConfiguration.MinimumLevel <= LogLevel.Debug)
                InternalLog(logEntry, LogLevel.Debug);
        }

        /// <summary>
        /// Log an message with log level debug
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message)
        {
            if (LoggingConfiguration == null || LoggingConfiguration.MinimumLevel <= LogLevel.Debug)
                InternalLog(new LogEntry { Message = message }, LogLevel.Debug);
        }

        /// <summary>
        /// Log an message consisting of args with log level info
        /// </summary>
        /// <param name="logEntry"></param>
        public void Info(LogEntry logEntry)
        {
            if (LoggingConfiguration == null || LoggingConfiguration.MinimumLevel <= LogLevel.Info)
                InternalLog(logEntry, LogLevel.Info);
        }

        /// <summary>
        /// Log an message consisting of args with log level info
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
	        if (LoggingConfiguration == null || LoggingConfiguration.MinimumLevel <= LogLevel.Info)
		        InternalLog(new LogEntry { Message = message }, LogLevel.Info);
        }


        /// <summary>
        /// Log an message consisting of args with log level warn
        /// </summary>
        /// <param name="logEntry"></param>
        public void Warn(LogEntry logEntry)
        {
            if (LoggingConfiguration == null || LoggingConfiguration.MinimumLevel <= LogLevel.Warn)
                InternalLog(logEntry, LogLevel.Warn);
        }

        /// <summary>
        /// Log an message consisting of args with log level warn
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message)
        {
	        if (LoggingConfiguration == null || LoggingConfiguration.MinimumLevel <= LogLevel.Warn)
		        InternalLog(new LogEntry { Message = message }, LogLevel.Warn);
        }

        /// <summary>
        /// Log an message consisting of args with log level error
        /// </summary>
        /// <param name="logEntry"></param>
        public void Error(LogEntry logEntry)
        {
            if (LoggingConfiguration == null || LoggingConfiguration.MinimumLevel <= LogLevel.Error)
                InternalLog(logEntry, LogLevel.Error);
        }

        /// <summary>
        /// Log an message consisting of args with log level error
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
	        if (LoggingConfiguration == null || LoggingConfiguration.MinimumLevel <= LogLevel.Error)
		        InternalLog(new LogEntry { Message = message }, LogLevel.Error);
        }


        /// <summary>
        /// Log an exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Exception(string message, Exception exception)
        {
            if (LoggingConfiguration == null || LoggingConfiguration.MinimumLevel <= LogLevel.Fatal)
                InternalLog(LogEntry.Exception(exception, message), LogLevel.Fatal);
        }


        /// <summary>
        /// Log an message consisting of args with log level fatal
        /// </summary>
        /// <param name="logEntry"></param>
        public void Fatal(LogEntry logEntry)
        {
            if (LoggingConfiguration == null || LoggingConfiguration.MinimumLevel <= LogLevel.Error)
                InternalLog(logEntry, LogLevel.Fatal);
        }

        /// <summary>
        /// Log an message consisting of args with log level fatal
        /// </summary>
        /// <param name="message"></param>
        public void Fatal(string message)
        {
	        if (LoggingConfiguration == null || LoggingConfiguration.MinimumLevel <= LogLevel.Fatal)
		        InternalLog(new LogEntry { Message = message }, LogLevel.Fatal);
        }

        /// <summary>
        /// Create string description of the log entry
        /// </summary>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        public string DescribeLogEntry(LogEntry logEntry)
        {
            if (logEntry == null) return null;
            BeautifyActionData(logEntry);
            var message = CreateMessage(logEntry);
            return message;
        }

        /// <summary>
        /// Internal log mechanism for logging log entries to log files and database
        /// </summary>
        /// <param name="logEntry"></param>
        /// <param name="level"></param>
        private void InternalLog(LogEntry logEntry, LogLevel level)
        {
            if (logEntry == null) return;

            logEntry.Level = level.ToString();
            BeautifyActionData(logEntry);
            var message = CreateMessage(logEntry);

            var logger = Logger
                .ForContext(nameof(logEntry.Level), logEntry.Level)
                .ForContext(nameof(logEntry.ErrorGroup), logEntry.ErrorGroup)
                .ForContext(nameof(logEntry.ErrorCode), logEntry.ErrorCode)
                .ForContext(nameof(logEntry.Origin), logEntry.Origin)
                .ForContext(nameof(logEntry.Action), logEntry.Action)
                .ForContext(nameof(logEntry.ProcessName), logEntry.ProcessName)
                .ForContext(nameof(logEntry.ActionFilePath), logEntry.ActionFilePath)
                .ForContext(nameof(logEntry.ActionSourceLineNumber), logEntry.ActionSourceLineNumber)
                .ForContext(nameof(logEntry.Message), logEntry.Message)
                .ForContext(nameof(logEntry.ExceptionMessage), logEntry.ExceptionMessage)
                .ForContext(nameof(logEntry.UserIdentifier), logEntry.UserIdentifier)
                .ForContext(nameof(logEntry.Payload), logEntry.Payload);

            logger.Write(level.AsSerilog(), message);
        }

        /// <summary>
        /// Return default value for a property
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private string GetValue(string value, string defaultValue = @"N/A")
        {
            return value ?? defaultValue;
        }

        /// <summary>
        /// Get exception message from log entry
        /// </summary>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        private string GetExceptionMessage(LogEntry logEntry)
        {
            if (string.IsNullOrEmpty(logEntry.ExceptionObject?.Message)) return null;

            logEntry.Message = logEntry.Message ?? string.Empty;
            string exceptionMessages = logEntry.ExceptionObject.ExtractMessages();
            string message = $"Base:{exceptionMessages}{Environment.NewLine}Stack:{logEntry.ExceptionObject.StackTrace}";
            message = message.CovertNewlinesToSpace();
            return message;
        }

        /// <summary>
        /// Get payload as message. Serialize the payload in JSON
        /// </summary>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        private string GetPayloadMessage(LogEntry logEntry)
        {
            if (logEntry.Payload == null) return @"NULL";
            var message = logEntry.Payload.Serialize(false);
            return message;
        }

        /// <summary>
        /// Parse mathod name from delegates, expressions, func and actions
        /// </summary>
        /// <param name="logEntry"></param>
        private void BeautifyActionData(LogEntry logEntry)
        {
            var match = logEntry.Action.GetMatch(@"<(.*)>");
            if (match.Success)
            {
                logEntry.Action = match.Value.Replace(@"<", string.Empty).Replace(@">", string.Empty);
            }
        }

        /// <summary>
        /// Create descriptive message for logging
        /// </summary>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        private string CreateMessage(LogEntry logEntry)
        {
            var exceptionMessage = GetExceptionMessage(logEntry);
            var payloadMessage = GetPayloadMessage(logEntry);
            var message = string.Format(@"{0:yyyy-MM-dd HH:mm:ss} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}",
                                        logEntry.CreatedOn,
                                        GetValue(logEntry.Level),
                                        GetValue(logEntry.ErrorCode),
                                        GetValue(logEntry.ErrorGroup),
                                        GetValue(logEntry.Origin),
                                        GetValue(logEntry.UserIdentifier),
                                        GetValue(logEntry.Action),
                                        GetValue(logEntry.ProcessName),
                                        GetValue(logEntry.Message),
                                        GetValue(exceptionMessage),
                                        GetValue(payloadMessage));
            return message;
        }
    }
}
