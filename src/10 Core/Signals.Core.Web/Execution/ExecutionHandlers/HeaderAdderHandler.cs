using Signals.Aspects.Bootstrap;
using Signals.Core.Business.Base;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Behaviour;
using Signals.Core.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Core.Web.Execution.ExecutionHandlers
{

    public class HeaderAdderHandler : IResultHandler
    {
        public MiddlewareResult HandleAfterExecution<TProcess>(TProcess process, Type type, VoidResult response, IHttpContextWrapper context) where TProcess : IBaseProcess<VoidResult>
        {
            var headers = SystemBootstrapper.GetInstance<List<ResponseHeaderAttribute>>();
            headers.ForEach(header =>
            {
                context.Headers.AddToResponse(header.Name, header.Value);
            });

            var headerAttribute = type
                .GetCustomAttributes(true)
                .Where(x => x.GetType() == typeof(ResponseHeaderAttribute) || x.GetType().IsSubclassOf(typeof(ResponseHeaderAttribute)))
                .Cast<ResponseHeaderAttribute>()
                .ToList();

            headerAttribute.ForEach(header =>
            {
                context.Headers.AddToResponse(header.Name, header.Value);
            });

            return MiddlewareResult.DoNothing;
        }
    }
}
