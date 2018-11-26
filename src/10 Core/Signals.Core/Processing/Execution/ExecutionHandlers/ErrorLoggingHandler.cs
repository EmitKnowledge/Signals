using Signals.Aspects.Bootstrap;
using Signals.Aspects.Logging;
using Signals.Core.Processes.Base;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Configuration;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Results;
using System;
using System.Linq;

namespace Signals.Core.Processing.Execution.ExecutionHandlers
{
    /// <summary>
    /// Process execution handler
    /// </summary>
    internal class ErrorLoggingHandler : IExecutionHandler
    {
        /// <summary>
        /// Next handler in execution pipe
        /// </summary>
        public IExecutionHandler Next { get; set; }

        /// <summary>
        /// Execute handler
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="process"></param>
        /// <param name="processType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public TResult Execute<TResult>(IBaseProcess<TResult> process, Type processType, params object[] args) where TResult : VoidResult, new()
        {
            var innerResult = Next.Execute(process, processType, args);
            var logger = SystemBootstrapper.GetInstance<ILogger>();
            if (logger.IsNull()) return innerResult;

            if (innerResult.IsFaulted)
            {
                if (innerResult.IsSystemFault)
                {
                    var errorInfo = innerResult.ErrorMessages.OfType<UnmanagedExceptionErrorInfo>().SingleOrDefault();
                    var exception = errorInfo.Exception;
                    var entry = LogEntry.Exception(exception, message: innerResult.GetFaultMessage());

                    entry.Action = exception.TargetSite.Name;
                    entry.Origin = ApplicationConfiguration.Instance?.ApplicationName ?? Environment.MachineName;
                    entry.Payload = args.SerializeJson();
                    entry.UserIdentifier = process.BaseContext.CurrentUserPrincipal?.Identity?.Name;

                    logger.Fatal(entry);
                }
                else
                {
                    var entry = LogEntry.Trace(message: innerResult.GetFaultMessage(), payload: args);
                    entry.Action = process.Name;
                    entry.Origin = ApplicationConfiguration.Instance?.ApplicationName ?? Environment.MachineName;
                    entry.Payload = args.SerializeJson();
                    entry.UserIdentifier = process.BaseContext.CurrentUserPrincipal?.Identity?.Name;

                    logger.Info(entry);
                }
            }

            return innerResult;
        }
    }
}
