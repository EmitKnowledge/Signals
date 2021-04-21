using Signals.Core.Common.Reflection;
using Signals.Core.Common.Serialization;
using Signals.Core.Processes.Api;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Helpers;
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

            if (correctMethod)
            {
                var statusCode = response.ToStatusCode();

                if (!response.IsFaulted)
                {
                    var responseType = response.GetType();
                    var responseResultType = responseType.GetGenericArguments().FirstOrDefault();
                    if (responseResultType == null)
                    {
                        return MiddlewareResult.DoNothing;
                    }

                    if (responseType.GetGenericTypeDefinition() == typeof(MethodResult<>))
                    {
                        var resultPropName = nameof(MethodResult<string>.Result);
                        var resultValue = responseType.GetProperty(resultPropName)?.GetValue(response, null);
                        if (resultValue != null)
                        {
                            var resultValueSerialized = responseResultType.IsSimpleType()
                                ? resultValue.ToString()
                                : resultValue.SerializeJson();

                            context.PutResponse(new HttpResponseMessage(statusCode)
                            {
                                Content = new StringContent(resultValueSerialized, Encoding.UTF8)
                            });
                            return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;
                        }
                    }
                }
            }

            // result is not handled, continue
            return MiddlewareResult.DoNothing;
        }
    }
}
