using Signals.Core.Processing.Results;
using Signals.Core.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Signals.Core.Common.Reflection;
using Signals.Core.Common.Serialization;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processes.Api;
using Signals.Core.Web.Helpers;

namespace Signals.Core.Web.Execution.ExecutionHandlers
{
    /// <summary>
    /// Xml result handler
    /// </summary>
    public class XmlResultHandler : IResultHandler
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
            var attributes = type.GetCachedAttributes<SignalsApiAttribute>();
            if (!attributes.Any()) return MiddlewareResult.DoNothing;

            var correctMethod = false;
            foreach (var attribute in attributes)
            {
                correctMethod |= attribute.ResponseType == SerializationFormat.Xml;
            }

            if (!correctMethod) return MiddlewareResult.DoNothing;

            var statusCode = response.IsSystemFault ? System.Net.HttpStatusCode.InternalServerError :
	            response.IsFaulted ? System.Net.HttpStatusCode.BadRequest : System.Net.HttpStatusCode.OK;

            context.PutResponse(new HttpResponseMessage(statusCode)
            {
	            Content = type.ToHttpContent(response)
            });

            this.D($"Returning XML result -> Status code: {statusCode}. Exit Handler.");

            return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;
        }
    }
}
