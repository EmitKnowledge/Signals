using Signals.Core.Processes.Base;
using Signals.Core.Processing.Authentication;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Results;
using System;
using System.Linq;
using Signals.Core.Common.Reflection;

namespace Signals.Core.Processing.Execution.ExecutionHandlers
{
    /// <summary>
    /// Process execution handler
    /// </summary>
    internal class AuthenticationHandler : IExecutionHandler
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
            // Get authenticate attribute
            var attributes = processType.GetCachedAttributes<SignalsAuthenticateAttribute>();

            // If no attribute is present the request is valid
            if (!attributes.Any()) return Next.Execute(process, processType, args);

            var correctMethod = false;
            foreach (var attribute in attributes)
            {
                // Try authenticate the user
                correctMethod |= attribute.Authenticate();
            }

            if (!correctMethod)
            {
                return VoidResult.Fail<TResult>(new AuthenticationErrorInfo());
            }

            var result = Next.Execute(process, processType, args);
            this.D($"Executed -> Authentication Handler for process type: {processType?.FullName}.");
            return result;
        }
    }
}
