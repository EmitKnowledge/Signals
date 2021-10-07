using Signals.Aspects.DI;
using Signals.Core.Processes.Api;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Behaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Reflection;

namespace Signals.Core.Web.Execution.ExecutionHandlers
{

    /// <summary>
    /// Result handler that adds http response headers based on attributes and filters
    /// </summary>
    public class AllowHeaderAdderHandler : IResultHandler
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
            var headers = SystemBootstrapper.GetInstance<List<ResponseHeaderAttribute>>();

            foreach (var header in headers)
            {
                foreach (var responseHeader in header.Headers)
                {
                    context.Headers.AddToResponse(responseHeader.Key, responseHeader.Value);
                }
            }

            var corsHeaderAttribute = type.GetCachedAttributes<CorsAttribute>();

            if (corsHeaderAttribute.IsNullOrHasZeroElements())
            {
                List<SignalsApiMethod> allowedMethods = new List<SignalsApiMethod>();

                var methodAttributes = type.GetCachedAttributes<SignalsApiAttribute>();
                if (methodAttributes.IsNullOrHasZeroElements())
                    allowedMethods.Add(SignalsApiMethod.ANY);
                else
                {
                    foreach (var attribute in methodAttributes)
                    {
                        allowedMethods.Add(attribute.HttpMethod);
                    }
                }

                allowedMethods.Remove(SignalsApiMethod.HEAD);
                allowedMethods.Remove(SignalsApiMethod.OPTIONS);

                if (allowedMethods.Contains(SignalsApiMethod.ANY))
                {
                    allowedMethods.Add(SignalsApiMethod.GET);
                    allowedMethods.Add(SignalsApiMethod.POST);
                    allowedMethods.Add(SignalsApiMethod.PUT);
                    allowedMethods.Add(SignalsApiMethod.PATCH);
                    allowedMethods.Add(SignalsApiMethod.DELETE);
                    allowedMethods.Remove(SignalsApiMethod.ANY);
                }

                context.Headers.AddToResponse("Allow", string.Join(", ", allowedMethods.Select(x => x.ToString())));
            }
            else
            {
                foreach (var header in corsHeaderAttribute)
                {
                    foreach (var responseHeader in header.Headers)
                    {
                        context.Headers.AddToResponse(responseHeader.Key, responseHeader.Value);
                    }
                }
            }

            return MiddlewareResult.DoNothing;
        }
    }
}