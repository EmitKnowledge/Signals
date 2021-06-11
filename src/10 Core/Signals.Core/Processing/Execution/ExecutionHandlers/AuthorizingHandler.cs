using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signals.Core.Common.Reflection;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Authorization;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processing.Execution.ExecutionHandlers
{
    /// <summary>
    /// Process execution handler
    /// </summary>
    internal class AuthorizingHandler : IExecutionHandler
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
            // get authorize attrigute
            var attributes = processType.GetCachedAttributes<SignalsAuthorizeAttribute>();

            // if no attribute is present the request is valid
            if (!attributes.Any()) return Next.Execute(process, processType, args);

            var correctMethod = false;
            foreach (var attribute in attributes)
            {
                // try authorize the user
                correctMethod |= attribute.Authorize(processType.Name);
            }

            if (!correctMethod)
            {
                return VoidResult.FaultedResult<TResult>(new AuthorizationErrorInfo());
            }

            return Next.Execute(process, processType, args);
        }
    }
}
