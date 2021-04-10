using System;
using System.Collections.Generic;
using System.Text;
using Signals.Aspects.DI;
using Signals.Aspects.ErrorHandling;
using Signals.Core.Processes.Base;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processing.Execution.ExecutionHandlers
{
    /// <summary>
    /// Process execution handler
    /// </summary>
    internal class ErrorManagingHandler : IExecutionHandler
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
            var strategyHandler = SystemBootstrapper.GetInstance<IStrategyHandler>();
            if (strategyHandler.IsNull() || !strategyHandler.AutoHandleErrorProcesses)
            {
                try
                {
                    return Next.Execute(process, processType, args);
                }
                catch (Exception ex)
                {
                    return VoidResult.FaultedResult<TResult>(ex);
                }
            }

            var strategyResult = strategyHandler.Execute(() => Next.Execute(process, processType, args));

            if (strategyResult.Exception != null)
            {
                return VoidResult.FaultedResult<TResult>(strategyResult.Exception);
            }

            return strategyResult.Result;
        }
    }
}
