using Signals.Aspects.DI;
using Signals.Aspects.Logging;
using Signals.Core.Processes.Base;
using Signals.Core.Common.Instance;
using Signals.Core.Configuration;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Signals.Core.Common.Serialization;

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
                // Create the log entry based on the fault type
                var errorInfo = innerResult.ErrorMessages.OfType<UnmanagedExceptionErrorInfo>().FirstOrDefault();
                var entry = innerResult.IsSystemFault
                    ? LogEntry.Exception(errorInfo?.Exception, innerResult.GetFaultMessage())
                    : LogEntry.Trace(message: innerResult.GetFaultMessage(), payload: args);

                // Provide entry info
                entry.ProcessName = process.Name;
                entry.Action = DisplayExecutionStack(process.ExecutionStack);
                entry.Origin = ApplicationConfiguration.Instance?.ApplicationName ?? Environment.MachineName;
                entry.Payload = GetExecutionStackPayload(process.ExecutionStack)?.SerializeJson();
                entry.UserIdentifier = process.BaseContext?.CurrentUserPrincipal?.Identity?.Name;

                // Log
                if (innerResult.IsSystemFault)
                {
                    logger.Fatal(entry);
                }
                else
                {
                    logger.Info(entry);
                }
            }

            return innerResult;
        }



        /// <summary>
        /// Displays the execution stack in format A -> B -> C
        /// </summary>
        /// <returns></returns>
        private string DisplayExecutionStack(Stack stack)
        {
            var enumerator = stack.GetEnumerator();
            var processes = new List<string>();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null)
                {
                    var stackEntry = enumerator.Current as ProcessExecutionStackEntry;
                    processes.Add(stackEntry?.Process?.Name);
                }
            }

            processes.Reverse();
            return string.Join(" -> ", processes);
        }

        /// <summary>
        /// Returns serialized payload of all processes in the execution stack
        /// </summary>
        /// <returns></returns>
        private List<object> GetExecutionStackPayload(Stack stack)
        {
            var enumerator = stack.GetEnumerator();
            var payloads = new List<object>();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null)
                {
                    var stackEntry = enumerator.Current as ProcessExecutionStackEntry;
                    payloads.Add(stackEntry?.Payload);
                }
            }

            payloads.Reverse();

            return payloads;
        }
    }
}
