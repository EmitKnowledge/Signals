using Signals.Aspects.Logging.Enums;
using System;
using System.Runtime.CompilerServices;

namespace Signals.Aspects.Logging
{
    /// <summary>
    /// Represent error log entry
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Unique id of the entity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Indicates when the records have been created in the system
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Level of information/Level of priority (INFO, DEBUG, TRACE, WARN...)
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// How this log entry is classified (Business process, Validation error, System error)
        /// </summary>
        public string ErrorGroup { get; set; }

        /// <summary>
        /// Indicate the error number of that occured
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Hold the value of the caller which created the log entry to be written (WEB APP, PLUGIN, BACKGROUND SERVICES)
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Actual method which caused the error (Register user, Create page)
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Name of execution process
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// Indicate the location of the faulted .cs file
        /// </summary>
        public string ActionFilePath { get; set; }

        /// <summary>
        /// Indicate the line number of the faulted file
        /// </summary>
        public string ActionSourceLineNumber { get; set; }

        /// <summary>
        /// Readable error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Exception.ToString() message
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Exception message
        /// </summary>
        public Exception ExceptionObject { get; set; }

        /// <summary>
        /// User who caused a log entry to be created
        /// </summary>
        public string UserIdentifier { get; set; }

        /// <summary>
        /// Associated data from the call
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public LogEntry()
        {
            CreatedOn = DateTime.UtcNow;
        }

        /// <summary>
        /// Reset captured source details. 
        /// Used when incorect information is captured to override with manual values
        /// </summary>
        public void ResetSourceDetails()
        {
            Action = null;
            ActionFilePath = null;
            ActionSourceLineNumber = null;
        }

        /// <summary>
        /// Set the payload data to the log entry
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload"></param>
        /// <param name="serializationFunction"></param>
        public void SetPayload<T>(T payload, Func<T, string> serializationFunction)
        {
            if (payload == null) return;
	        Payload = serializationFunction?.Invoke(payload);
		}

        /// <summary>
        /// Exception log entry
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        /// <param name="serializationFunction"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        /// <returns></returns>
        public static LogEntry Exception(Exception ex = null,
                                     string message = null,
                                     dynamic payload = null,
                                     Func<dynamic, string> serializationFunction = null,
                                     [CallerMemberName] string memberName = "",
                                     [CallerFilePath] string sourceFilePath = "",
                                     [CallerLineNumber] int sourceLineNumber = 0)
        {
            var log = CreateLogEntry(ex, message, payload, serializationFunction, memberName, sourceFilePath, sourceLineNumber);
            log.Level = LogLevel.Fatal.ToString();
            return log;
        }

        /// <summary>
        /// Info log entry
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        /// <param name="serializationFunction"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        /// <returns></returns>
        public static LogEntry Info(Exception ex = null,
                                     string message = null,
                                     dynamic payload = null,
                                     Func<dynamic, string> serializationFunction = null,
                                     [CallerMemberName] string memberName = "",
                                     [CallerFilePath] string sourceFilePath = "",
                                     [CallerLineNumber] int sourceLineNumber = 0)
        {
            var log = CreateLogEntry(ex, message, payload, serializationFunction, memberName, sourceFilePath, sourceLineNumber);
            log.Level = LogLevel.Info.ToString();
            return log;
        }

        /// <summary>
        /// Debug log entry
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        /// <param name="serializationFunction"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        /// <returns></returns>
        public static LogEntry Debug(Exception ex = null,
                                     string message = null,
                                     dynamic payload = null,
                                     Func<dynamic, string> serializationFunction = null,
                                     [CallerMemberName] string memberName = "",
                                     [CallerFilePath] string sourceFilePath = "",
                                     [CallerLineNumber] int sourceLineNumber = 0)
        {
            var log = CreateLogEntry(ex, message, payload, serializationFunction, memberName, sourceFilePath, sourceLineNumber);
            log.Level = LogLevel.Debug.ToString();
            return log;
        }

        /// <summary>
        /// Trace log entry
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        /// <param name="serializationFunction"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        /// <returns></returns>
        public static LogEntry Trace(Exception ex = null,
                                     string message = null,
                                     dynamic payload = null,
                                     Func<dynamic, string> serializationFunction = null,
                                     [CallerMemberName] string memberName = "",
                                     [CallerFilePath] string sourceFilePath = "",
                                     [CallerLineNumber] int sourceLineNumber = 0)
        {
            var log = CreateLogEntry(ex, message, payload, serializationFunction, memberName, sourceFilePath, sourceLineNumber);
            log.Level = LogLevel.Trace.ToString();
            return log;
        }

        /// <summary>
        /// Warning log entry
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        /// <param name="serializationFunction"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        /// <returns></returns>
        public static LogEntry Warn(Exception ex = null,
                                     string message = null,
                                     dynamic payload = null,
                                     Func<dynamic, string> serializationFunction = null,
                                     [CallerMemberName] string memberName = "",
                                     [CallerFilePath] string sourceFilePath = "",
                                     [CallerLineNumber] int sourceLineNumber = 0)
        {
            var log = CreateLogEntry(ex, message, payload, serializationFunction, memberName, sourceFilePath, sourceLineNumber);
            log.Level = LogLevel.Warn.ToString();
            return log;
        }

        /// <summary>
        /// Create log entry
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        /// <param name="serializationFunction"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        /// <returns></returns>
        private static LogEntry CreateLogEntry(Exception ex,
                                                  string message,
                                                  dynamic payload,
                                                  Func<dynamic, string> serializationFunction,
                                                  string memberName,
                                                  string sourceFilePath,
                                                  int sourceLineNumber)
        {
            var logEntry = new LogEntry();
            logEntry.ExceptionObject = ex;
            logEntry.ExceptionMessage = ex?.ToString();
            logEntry.Message = message?.ToUpper();
            logEntry.Action = memberName;
            logEntry.ActionFilePath = sourceFilePath;
            logEntry.ActionSourceLineNumber = sourceLineNumber.ToString();
            logEntry.SetPayload(payload, serializationFunction);

            return logEntry;
        }
    }
}
