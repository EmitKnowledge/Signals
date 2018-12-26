using Signals.Core.Common.Serialization;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Helpers;
using System;
using System.Linq;
using System.Net.Http;

namespace Signals.Core.Web.Execution.ExecutionHandlers.FailedExecution
{
    /// <summary>
    /// Authorization fail result handler
    /// </summary>
    public class AuthorizationFailResultFilter : IResultHandler
    {
        /// <summary>
        /// Handle process result
        /// </summary>
        /// <typeparam name="TProcess"></typeparam>
        /// <param name="process"></param>
        /// <param name="type"></param>
        /// <param name="response"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public MiddlewareResult HandleAfterExecution<TProcess>(TProcess process, Type type, VoidResult response, IHttpContextWrapper context) where TProcess : IBaseProcess<VoidResult>
        {
            if (response.IsFaulted && response.ErrorMessages.OfType<AuthorizationErrorInfo>().Any())
            {
                context.PutResponse(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                {
                    Content = type.ToHttpContent(response)
                });
                return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;
            }

            return MiddlewareResult.DoNothing;
        }
    }
}
