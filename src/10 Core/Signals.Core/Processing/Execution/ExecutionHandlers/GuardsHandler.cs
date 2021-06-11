using Signals.Core.Common.Reflection;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Guards;
using Signals.Core.Processing.Results;
using System;
using System.Linq;

namespace Signals.Core.Processing.Execution.ExecutionHandlers
{
    /// <summary>
    /// Process execution handler
    /// </summary>
    internal class GuardsHandler : IExecutionHandler
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
            // Get guard attribute
            var guardAttributes = processType.GetCachedAttributes<SignalsGuardAttribute>();
            foreach (var guardAttribute in guardAttributes)
            {
                var guardResult = guardAttribute.Check(process.BaseContext);
                if (guardResult != null)
                {
                    return VoidResult.FaultedResult<TResult>(guardResult);
                }
            }

            return Next.Execute(process, processType, args);
        }
    }
}
