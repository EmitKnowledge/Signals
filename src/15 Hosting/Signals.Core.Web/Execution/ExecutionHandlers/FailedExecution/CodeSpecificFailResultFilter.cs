﻿using Signals.Core.Processes.Base;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Helpers;
using System;
using System.Linq;

namespace Signals.Core.Web.Execution.ExecutionHandlers.FailedExecution
{
    /// <summary>
    /// General fail result handler
    /// </summary>
    public class CodeSpecificFailResultFilter : IResultHandler
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
	        if (!response.IsFaulted || !response.ErrorMessages.OfType<CodeSpecificErrorInfo>().Any())
	        {
		        return MiddlewareResult.DoNothing;
	        }

	        var error = response.ErrorMessages.OfType<CodeSpecificErrorInfo>().First();
            context.PutResponse(new System.Net.Http.HttpResponseMessage(error.HttpStatusCode)
            {
	            Content = type.ToHttpContent(response)
            });

            this.D($"Code Specific Error -> Status code: {error.HttpStatusCode} -> Error: {error.FaultMessage}. Exit Filter.");

            return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;
        }
    }
}
