using Signals.Core.Processing.Input.Http;
using Signals.Core.Web.Http;
using System;

namespace Signals.Core.Web.Execution.Filters
{
    /// <summary>
    /// Process type filter
    /// </summary>
    public class RouteFilter : IFilter
    {
        /// <summary>
        /// Check if the process type is correct based on the request
        /// </summary>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsCorrectProcessType(Type type, IHttpContextWrapper context)
        {
            var assemblyNamespace = type.Assembly.FullName.Split(',')[0];

            var path = ("/api/" +
                type
                .FullName
                .Replace(assemblyNamespace, "")
                .Replace('.', '/'))
                .Replace("//", "/")
                .Trim('/')
                .ToLowerInvariant();

            var url = context
                .RawUrl
                .Trim('/')
                .ToLowerInvariant();

            return path == url;
        }
    }
}
