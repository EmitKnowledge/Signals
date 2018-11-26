using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Http;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Signals.Core.Common.Reflection;
using Signals.Core.Processing.Input.Http;

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
            if (response is FileResult fileResponse)
            {
                // create response
                var httpRespose = new HttpResponseMessage
                {
                    Content = new StreamContent(fileResponse.Result),
                };

                httpRespose.Content.Headers.ContentType = new MediaTypeHeaderValue(fileResponse.MimeType);
                httpRespose.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = fileResponse.FileName };
                context.PutResponse(httpRespose);

                // stop handling pipe
                return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;
            }

            // result is not handled, continue
            return MiddlewareResult.DoNothing;
        }
    }
}
