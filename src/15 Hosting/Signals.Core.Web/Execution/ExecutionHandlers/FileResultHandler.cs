using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Common.Instance;

namespace Signals.Core.Web.Execution.ExecutionHandlers
{
    /// <summary>
    /// File result handler
    /// </summary>
    public class FileResultHandler : IResultHandler
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
	        if (!(response is FileResult fileResponse))
	        {
		        return MiddlewareResult.DoNothing;
	        }

	        var statusCode = response.IsSystemFault ? System.Net.HttpStatusCode.InternalServerError :
	            response.IsFaulted ? System.Net.HttpStatusCode.BadRequest : System.Net.HttpStatusCode.OK;

            // create response
            var httpRespose = new HttpResponseMessage(statusCode)
            {
	            Content = fileResponse.IsNull() ? null : new StreamContent(fileResponse.Result),
            };

            httpRespose.Content.Headers.ContentType = new MediaTypeHeaderValue(fileResponse.MimeType);
            httpRespose.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = fileResponse.FileName };
            context.PutResponse(httpRespose);

            this.D($"File result with -> Mime Type: {fileResponse?.MimeType} -> Filename: {fileResponse?.FileName} has been set to response. Status code: {statusCode}. Exit Handler.");
            // stop handling pipe
            return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;
        }
    }
}
