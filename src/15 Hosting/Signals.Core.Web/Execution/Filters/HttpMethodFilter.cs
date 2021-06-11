using System;
using System.Linq;
using Signals.Core.Common.Reflection;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Input.Http;

namespace Signals.Core.Web.Execution.Filters
{
    /// <summary>
    /// Process type filter
    /// </summary>
    public class HttpMethodFilter : IFilter
    {
        /// <summary>
        /// Check if the process type is correct based on the request
        /// </summary>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsCorrectProcessType(Type type, IHttpContextWrapper context)
        {
            if (context.HttpMethod == SignalsApiMethod.OPTIONS || context.HttpMethod == SignalsApiMethod.HEAD) return true;

            var attributes = type.GetCachedAttributes<SignalsApiAttribute>();
            if (!attributes.Any()) return true;

            var correctMethod = false;
            foreach (var attribute in attributes)
            {
                correctMethod |= attribute.HttpMethod == SignalsApiMethod.ANY || attribute.HttpMethod == context.HttpMethod;
            }

            return correctMethod;
        }
    }
}
