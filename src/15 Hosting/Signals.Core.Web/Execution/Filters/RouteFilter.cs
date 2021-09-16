using Signals.Core.Processing.Input.Http;
using Signals.Core.Web.Http;
using System;
using System.Collections.Concurrent;

namespace Signals.Core.Web.Execution.Filters
{
    /// <summary>
    /// Process type filter
    /// </summary>
    public class RouteFilter : IFilter
    {
	    /// <summary>
	    /// Cache all existing paths
	    /// If this filter return true, cache the path
	    /// </summary>
	    private static readonly ConcurrentDictionary<string, string> TypePathRegistry = new ConcurrentDictionary<string, string>();
	    
        /// <summary>
        /// Check if the process type is correct based on the request
        /// </summary>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsCorrectProcessType(Type type, IHttpContextWrapper context)
        {
	        var path = TypePathRegistry.GetOrAdd(type.FullName, _ =>
	        {
		        var assemblyNamespace = type.Assembly.FullName.Split(',')[0];
		        var typeName = type?.FullName?.Replace(assemblyNamespace, "").Replace('.', '/');
                return $"/api/{typeName}".Replace("//", "/").Trim('/');
            });

	        var areEqual = string.Compare(path, context?.RawUrl?.Trim('/'), StringComparison.InvariantCultureIgnoreCase) == 0;
	        return areEqual;
        }
    }
}
