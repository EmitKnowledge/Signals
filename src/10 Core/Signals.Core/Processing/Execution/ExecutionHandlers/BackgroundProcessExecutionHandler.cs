using System;
using System.Collections.Generic;
using System.Text;
using Signals.Core.Processes.Base;
using Signals.Core.Processes.Distributed;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processing.Execution.ExecutionHandlers
{
    /// <summary>
    /// Process execution handler
    /// </summary>
    internal class BackgroundProcessExecutionHandler : IExecutionHandler
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
            if(process is IDistributedProcess distributedProcess)
                return distributedProcess.ExecuteBackgroundProcess(args) as TResult;

            return null;
        }
    }
}
