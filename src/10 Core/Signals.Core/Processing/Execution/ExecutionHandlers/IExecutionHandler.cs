using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;
using System;

namespace Signals.Core.Processing.Execution.ExecutionHandlers
{
    /// <summary>
    /// Process execution handler
    /// </summary>
    internal interface IExecutionHandler
    {
        /// <summary>
        /// Next handler in execution pipe
        /// </summary>
        IExecutionHandler Next { get; set; }

        /// <summary>
        /// Execute handler
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="process"></param>
        /// <param name="processType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        TResult Execute<TResult>(IBaseProcess<TResult> process, Type processType, params object[] args) where TResult : VoidResult, new();
    }
}
