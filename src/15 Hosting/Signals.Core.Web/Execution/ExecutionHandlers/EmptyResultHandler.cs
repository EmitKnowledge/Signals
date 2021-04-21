
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Signals.Core.Processes.Base;
using Signals.Core.Common.Serialization;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Http;
using Signals.Core.Web.Helpers;

namespace Signals.Core.Web.Execution.ExecutionHandlers
{
    /// <summary>
    /// Json result handler
    /// </summary>
    public class EmptyResultHandler : IResultHandler
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
            var statusCode = response.ToStatusCode();

            context.PutResponse(new HttpResponseMessage(statusCode));

            // result is always handled, this is result fallback
            return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;
        }
    }
}
