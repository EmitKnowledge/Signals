using System;
using System.Collections.Generic;
using System.Text;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processing.Execution.ExecutionHandlers
{
    /// <summary>
    /// Process execution handler
    /// </summary>
    internal class CommonProcessExecutionHandler : IExecutionHandler
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
            var result = process.ExecuteProcess(args);
            this.D($"Executed -> Common Process Execution Handler for process type: {processType?.FullName}.");
            return result;
        }
    }
}
