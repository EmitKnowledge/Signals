using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using Signals.Aspects.Logging.NLog.Configurations;
using Signals.Aspects.Logging.NLog.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Signals.Aspects.Logging.NLog
{
    /// <summary>
    /// MLog logger implementation
    /// </summary>
    public class NLogger : ILogger
    {
	    /// <summary>
	    /// Initiate logging in file
	    /// </summary>
	    /// <param name="configuration"></param>
	    /// <param name="levels"></param>
	    public NLogger(FileLoggingConfiguration configuration = null, params LogLevel[] levels)
        {
            InitTarget(new FileTarget("file")
            {
                CreateDirs = true,
                ArchiveNumbering = ArchiveNumberingMode.Date,
                ArchiveEvery = FileArchivePeriod.Day,

                MaxArchiveFiles = configuration.MaxArchiveFiles,
                FileName = $"{configuration.LogFileDirectory}/${{shortdate}}.log",
                ArchiveFileName = $"{configuration.LogFileDirectory}/${{shortdate}}.log",
                Layout = configuration.MessageTemplate
            }, levels?.ToList());
        }


	    /// <summary>
	    /// Initiate logging in console
	    /// </summary>
	    /// <param name="configuration"></param>
	    /// <param name="levels"></param>
	    public NLogger(ConsoleLoggingConfiguration configuration, params LogLevel[] levels)
        {
            InitTarget(new ConsoleTarget("console")
            {
                DetectConsoleAvailable = true,
                Layout = configuration.MessageTemplate
            }, levels?.ToList());
        }

	    /// <summary>
	    /// Initiate logging in database
	    /// </summary>
	    /// <param name="configuration"></param>
	    /// <param name="levels"></param>
	    public NLogger(DatabaseLoggingConfiguration configuration, params LogLevel[] levels)
        {
            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction($"Create{configuration.TableName}"))
                {
                    var sql =
                        $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
                            FROM sys.tables t 
                            WHERE t.name = '{configuration.TableName}'
                        )
                        BEGIN
                            CREATE TABLE [{configuration.TableName}]
                            (
                                [Id] INT IDENTITY(1,1) NOT NULL, 
                                [CreatedOn] DATETIME2(7) NOT NULL, 
                                [Level] NVARCHAR(MAX) NULL,
                                [ErrorGroup] NVARCHAR(MAX) NULL,
                                [ErrorCode] NVARCHAR(MAX) NULL,
                                [Origin] NVARCHAR(MAX) NULL,
                                [ProcessName] NVARCHAR(MAX) NULL,
                                [Action] NVARCHAR(MAX) NULL,
                                [ActionFilePath] NVARCHAR(MAX) NULL,
                                [ActionSourceLineNumber] NVARCHAR(MAX) NULL,
                                [Message] NVARCHAR(MAX) NULL,
                                [ExceptionMessage] NVARCHAR(MAX) NULL,
                                [UserIdentifier] NVARCHAR(MAX) NULL,
                                [Payload] NVARCHAR(MAX) NULL
                            )
                        
                            ALTER TABLE [{configuration.TableName}] ADD CONSTRAINT [DF_{configuration.TableName}_CreateOn]  DEFAULT (getutcdate()) FOR [CreatedOn]; 
                        END
                    ";

                    var command = new SqlCommand(sql, connection, transaction);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }

            var dbTarget = new DatabaseTarget("database")
            {
                DBHost = configuration.Host,
                DBDatabase = configuration.Database,
                DBPassword = configuration.Password,
                DBUserName = configuration.Username,
                DBProvider = configuration.DataProvider.GetDescription(),
                CommandText = $@"
                    insert into {configuration.TableName} (
                        Level,
                        ErrorGroup,
                        ErrorCode,
                        Origin,
                        Action,
                        ProcessName,
                        ActionFilePath,
                        ActionSourceLineNumber,
                        Message,
                        ExceptionMessage,
                        UserIdentifier,
                        Payload
                    ) values (
                        @Level,
                        @ErrorGroup,
                        @ErrorCode,
                        @Origin,
                        @Action,
                        @ProcessName,
                        @ActionFilePath,
                        @ActionSourceLineNumber,
                        @Message,
                        @ExceptionMessage,
                        @UserIdentifier,
                        @Payload
                    );"
            };

            dbTarget.Parameters.Add(new DatabaseParameterInfo("@level", Layout.FromString(@"${event-properties:item=Level}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@errorGroup", Layout.FromString(@"${event-properties:item=ErrorGroup}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@errorCode", Layout.FromString(@"${event-properties:item=ErrorCode}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@origin", Layout.FromString(@"${event-properties:item=Origin}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@action", Layout.FromString(@"${event-properties:item=Action}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@processName", Layout.FromString(@"${event-properties:item=ProcessName}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@actionFilePath", Layout.FromString(@"${event-properties:item=ActionFilePath}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@actionSourceLineNumber", Layout.FromString(@"${event-properties:item=ActionSourceLineNumber}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@message", Layout.FromString(@"${event-properties:item=Message}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@exceptionMessage", Layout.FromString(@"${event-properties:item=ExceptionMessage}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@userIdentifier", Layout.FromString(@"${event-properties:item=UserIdentifier}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@payload", Layout.FromString(@"${event-properties:item=Payload}")));

            InitTarget(dbTarget, levels?.ToList());
        }


	    /// <summary>
	    /// Initiate logging from file configuration
	    /// </summary>
	    /// <param name="filePath"></param>
	    public NLogger(string filePath)
        {
            LogManager.Configuration = new XmlLoggingConfiguration(filePath, false);
        }

	    /// <summary>
	    /// Initiate logging target
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="levels"></param>
	    private void InitTarget(Target target, List<LogLevel> levels)
        {
            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            config.AddTarget(target);

            // Step 3. Define rules
            if (levels == null || levels.Count == 0)
            {
                config.AddRuleForAllLevels(target);
            }
            else
            {
                foreach (var level in levels)
                {
                    config.AddRuleForOneLevel(level, target);
                }
            }

            // Step 4. Activate the configuration
            LogManager.Configuration = config;
        }

        /// <summary>
        /// Log an message consisting of args with log level debug
        /// </summary>
        /// <param name="logEntry"></param>
        public void Debug(LogEntry logEntry)
        {
            InternalLog(logEntry, LogLevel.Debug);
        }

        /// <summary>
        /// Log an message with log level debug
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message)
        {
            InternalLog(new LogEntry { Message = message }, LogLevel.Debug);
        }

        /// <summary>
        /// Log an message consisting of args with log level error
        /// </summary>
        /// <param name="logEntry"></param>
        public void Error(LogEntry logEntry)
        {
            InternalLog(logEntry, LogLevel.Error);
        }

        /// <summary>
        /// Log an exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Exception(string message, Exception exception)
        {
            var logEntry = LogEntry.Exception(exception, message);
            InternalLog(logEntry, LogLevel.Fatal);
        }


        /// <summary>
        /// Log an message consisting of args with log level Error
        /// </summary>
        /// <param name="args"></param>
        public void Error(params object[] args)
        {
            InternalLog(new LogEntry { Payload = args?.Serialize() }, LogLevel.Error);
        }


        /// <summary>
        /// Log an message consisting of args with log level fatal
        /// </summary>
        /// <param name="logEntry"></param>
        public void Fatal(LogEntry logEntry)
        {
            InternalLog(logEntry, LogLevel.Fatal);
        }

        /// <summary>
        /// Log an message consisting of args with log level info
        /// </summary>
        /// <param name="logEntry"></param>
        public void Info(LogEntry logEntry)
        {
            InternalLog(logEntry, LogLevel.Info);
        }


        /// <summary>
        /// Log an message
        /// </summary>
        /// <param name="args"></param>
        public void Info(params object[] args)
        {
            InternalLog(new LogEntry { Payload = args?.Serialize() }, LogLevel.Info);
        }


        /// <summary>
        /// Log an message consisting of args with log level trace
        /// </summary>
        /// <param name="logEntry"></param>
        public void Trace(LogEntry logEntry)
        {
            InternalLog(logEntry, LogLevel.Trace);
        }

        /// <summary>
        /// Log an message consisting of args with log level warn
        /// </summary>
        /// <param name="logEntry"></param>
        public void Warn(LogEntry logEntry)
        {
            InternalLog(logEntry, LogLevel.Warn);
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

            var loggerName = logEntry?.Action ?? "default";
            var logger = LogManager.GetLogger(loggerName);

            var logEvent = new LogEventInfo(level, loggerName, message);

            logEvent.Properties[nameof(logEntry.Level)] = logEntry.Level;
            logEvent.Properties[nameof(logEntry.ErrorGroup)] = logEntry.ErrorGroup;
            logEvent.Properties[nameof(logEntry.ErrorCode)] = logEntry.ErrorCode;
            logEvent.Properties[nameof(logEntry.Origin)] = logEntry.Origin;
            logEvent.Properties[nameof(logEntry.Action)] = logEntry.Action;
            logEvent.Properties[nameof(logEntry.ProcessName)] = logEntry.ProcessName;
            logEvent.Properties[nameof(logEntry.ActionFilePath)] = logEntry.ActionFilePath;
            logEvent.Properties[nameof(logEntry.ActionSourceLineNumber)] = logEntry.ActionSourceLineNumber;
            logEvent.Properties[nameof(logEntry.Message)] = logEntry.Message;
            logEvent.Properties[nameof(logEntry.ExceptionMessage)] = logEntry.ExceptionMessage;
            logEvent.Properties[nameof(logEntry.UserIdentifier)] = logEntry.UserIdentifier;
            logEvent.Properties[nameof(logEntry.Payload)] = logEntry.Payload;
            

            logger.Log(logEvent);
        }

        /// <summary>
        /// Return default value for a propertu
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
