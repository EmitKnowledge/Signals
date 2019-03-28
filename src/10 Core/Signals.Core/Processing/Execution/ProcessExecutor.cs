using Signals.Core.Processes.Base;
using Signals.Core.Processes.Distributed;
using Signals.Core.Processing.Execution.ExecutionHandlers;
using Signals.Core.Processing.Results;
using System.Collections.Generic;

namespace Signals.Core.Processing.Execution
{
    /// <summary>
    /// Process executor
    /// </summary>
    internal interface IProcessExecutor
    {
        /// <summary>
        /// Execute process using passed arguments
        /// </summary>
        /// <param name="process"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        TResult Execute<TResult>(IBaseProcess<TResult> process, params object[] args) where TResult : VoidResult, new();

        /// <summary>
        /// Execute background process using passed arguments
        /// </summary>
        /// <param name="process"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        TResult ExecuteBackground<TResult>(IBaseProcess<TResult> process, params object[] args) where TResult : VoidResult, new();
    }

    /// <summary>
    /// Process executor impl
    /// </summary>
    internal class ProcessExecutor : IProcessExecutor
    {
        /// <summary>
        /// Execute process using passed arguments
        /// </summary>
        /// <param name="process"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public TResult Execute<TResult>(IBaseProcess<TResult> process, params object[] args) where TResult : VoidResult, new()
        {
            var pipe = new List<IExecutionHandler>
            {
                new AuthenticationHandler(),
                new AuthorizingHandler(),
                new ErrorLoggingHandler(),
                new AuditingHandler(),
                new ErrorManagingHandler(),
                new CriticalNotifyingHandler(),
                new CommonProcessExecutionHandler(),
            };

            for (int i = 0; i < pipe.Count - 1; i++)
                pipe[i].Next = pipe[i + 1];

            var processType = process.GetType();
            var result = pipe[0].Execute(process, processType, args);
            return result;
        }

        /// <summary>
        /// Execute process using passed arguments
        /// </summary>
        /// <param name="process"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public TResult ExecuteBackground<TResult>(IBaseProcess<TResult> process, params object[] args) where TResult : VoidResult, new()
        {
            var pipe = new List<IExecutionHandler>
            {
                new ErrorLoggingHandler(),
                new AuditingHandler(),
                new ErrorManagingHandler(),
                new CriticalNotifyingHandler(),
                new BackgroundProcessExecutionHandler(),
            };

            for (int i = 0; i < pipe.Count - 1; i++)
                pipe[i].Next = pipe[i + 1];

            var processType = process.GetType();
            var result = pipe[0].Execute(process, processType, args);
            return result;
        }
    }
}