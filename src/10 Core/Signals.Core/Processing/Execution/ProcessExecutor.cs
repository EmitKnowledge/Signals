using Signals.Core.Processes.Base;
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
        private List<IExecutionHandler> _foregroundPipe;
        private List<IExecutionHandler> _backgroundPipe;

        /// <summary>
        /// CTOR
        /// </summary>
        public ProcessExecutor()
        {
            _foregroundPipe = new List<IExecutionHandler>
            {
                new AuthenticationHandler(),
                new AuthorizingHandler(),
                new GuardsHandler(),
                new ErrorLoggingHandler(),
                new AuditingHandler(),
                new ErrorManagingHandler(),
                new CriticalNotifyingHandler(),
                new CommonProcessExecutionHandler(),
            };
            
            _backgroundPipe = new List<IExecutionHandler>
            {
                new ErrorLoggingHandler(),
                new AuditingHandler(),
                new ErrorManagingHandler(),
                new CriticalNotifyingHandler(),
                new BackgroundProcessExecutionHandler(),
            };


            for (int i = 0; i < _foregroundPipe.Count - 1; i++)
                _foregroundPipe[i].Next = _foregroundPipe[i + 1];

            for (int i = 0; i < _backgroundPipe.Count - 1; i++)
                _backgroundPipe[i].Next = _backgroundPipe[i + 1];
        }

        /// <summary>
        /// Execute process using passed arguments
        /// </summary>
        /// <param name="process"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public TResult Execute<TResult>(IBaseProcess<TResult> process, params object[] args) where TResult : VoidResult, new()
        {
            var processType = process.GetType();
            var result = _foregroundPipe[0].Execute(process, processType, args);
            process.ExecutionStack.Pop();
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
            var processType = process.GetType();
            var result = _backgroundPipe[0].Execute(process, processType, args);
            return result;
        }
    }
}