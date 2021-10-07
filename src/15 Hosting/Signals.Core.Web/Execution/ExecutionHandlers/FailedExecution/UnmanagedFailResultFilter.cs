using Signals.Core.Processes.Base;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Helpers;
using System;
using System.Linq;

namespace Signals.Core.Web.Execution.ExecutionHandlers.FailedExecution
{
    /// <summary>
    /// System fail result handler
    /// </summary>
    public class UnmanagedFailResultFilter : IResultHandler
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
	        if (!response.IsFaulted || !response.ErrorMessages.OfType<UnmanagedExceptionErrorInfo>().Any())
	        {
		        return MiddlewareResult.DoNothing;
	        }

	        context.PutResponse(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
	            Content = type.ToHttpContent(response)
            });

	        var error = response.ErrorMessages.OfType<UnmanagedExceptionErrorInfo>().FirstOrDefault();
	        this.D($"General Error -> Status code: {System.Net.HttpStatusCode.BadRequest} -> Error: {error?.Exception?.Message ?? "N/A"}. Exit Filter.");

            return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;

        }
    }
}
