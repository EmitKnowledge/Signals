using Signals.Core.Processes.Base;
using Signals.Core.Processing.Guards;
using Signals.Core.Processing.Results;
using System;
using System.Linq;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Reflection;

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
	            this.D($"Executed Guards Handler of -> Type: {guardAttribute.GetType().FullName} for process type: {processType?.FullName}.");
                if (guardResult == null) continue;
                return VoidResult.Fail<TResult>(guardResult);
            }

            var result = Next.Execute(process, processType, args);
            return result;
        }
    }
}
