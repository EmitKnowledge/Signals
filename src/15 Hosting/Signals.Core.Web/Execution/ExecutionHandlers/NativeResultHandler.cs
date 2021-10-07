using Signals.Core.Common.Reflection;
using Signals.Core.Common.Serialization;
using Signals.Core.Processes.Api;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Signals.Core.Web.Execution.ExecutionHandlers
{
    /// <summary>
    /// Native result handler
    /// </summary>
    public class NativeResultHandler : IResultHandler
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

            if (!attributes.Any())
            {
                return MiddlewareResult.DoNothing;
            }

            var correctMethod = false;
            foreach (var attribute in attributes)
            {
                correctMethod |= attribute.NativeResponse;
            }

            if (!correctMethod)
            {
	            return MiddlewareResult.DoNothing;
            }

            var statusCode = response.IsSystemFault ? System.Net.HttpStatusCode.InternalServerError :
	            response.IsFaulted ? System.Net.HttpStatusCode.BadRequest : System.Net.HttpStatusCode.OK;

            if (response.IsFaulted) return MiddlewareResult.DoNothing;
            
            var responseType = response.GetType();
            var responseResultType = responseType.GetGenericArguments().FirstOrDefault();
            if (responseResultType == null)
            {
	            this.D("Response type with generic argument not found. Exit Handler.");
                return MiddlewareResult.DoNothing;
            }

            if (responseType.GetGenericTypeDefinition() != typeof(MethodResult<>))
            {
	            this.D($"Native response must return MethodResult<> -> Provided Type: {responseType.GetGenericTypeDefinition()}. Exit Handler.");
                return MiddlewareResult.DoNothing;
            }

            var resultPropName = nameof(MethodResult<string>.Result);
            var resultValue = responseType.GetProperty(resultPropName)?.GetValue(response, null);
            if (resultValue == null)
            {
	            this.D("Response value is NULL. Exit handler.");
                return MiddlewareResult.DoNothing;
            }

            var resultValueSerialized = responseResultType.IsSimpleType()
	            ? resultValue.ToString()
	            : resultValue.SerializeJson();

            context.PutResponse(new HttpResponseMessage(statusCode)
            {
	            Content = new StringContent(resultValueSerialized, Encoding.UTF8)
            });

            this.D($"Native -> Response: {resultValueSerialized} returned. Exit handler.");

            return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;
        }
    }
}
