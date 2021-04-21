using Signals.Aspects.DI;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Reflection;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Behaviour;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Core.Web.Execution.ExecutionHandlers
{

    /// <summary>
    /// Result handler that adds http response headers based on attribiutes and filters
    /// </summary>
    public class HeaderAdderHandler : IResultHandler
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

            var headerAttribute = type.GetCachedAttributes<ResponseHeaderAttribute>();

            foreach (var header in headerAttribute)
            {
                foreach (var responseHeader in header.Headers)
                {
                    context.Headers.AddToResponse(responseHeader.Key, responseHeader.Value);
                }
            }

            return MiddlewareResult.DoNothing;
        }
    }
}
