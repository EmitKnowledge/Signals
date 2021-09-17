using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Web.Http;

namespace Signals.Core.Web.Execution.Filters
{
    /// <summary>
    /// Process type filter
    /// </summary>
    public class HttpMethodFilter : IFilter
    {
	    /// <summary>
	    /// Cache all processes that has the Signals API Attribute types that match
	    /// </summary>
	    private static readonly ConcurrentDictionary<string, bool> TypeApiAttributeRegistry = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Check if the process type is correct based on the request
        /// </summary>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsCorrectProcessType(Type type, IHttpContextWrapper context)
        {
			// if cached true, return it right away
	        TypeApiAttributeRegistry.TryGetValue(type.FullName, out var hasMatchingSignalsApiAttribute);
	        if (hasMatchingSignalsApiAttribute) return true;

			// otherwise check if it is matching
	        var attributes = type.GetCustomAttributes(typeof(SignalsApiAttribute), false).Cast<SignalsApiAttribute>().ToList();
	        if (!attributes.Any())
	        {
		        TypeApiAttributeRegistry.TryAdd(type.FullName, true);
				return true;
	        }

	        var correctMethod = false;
	        foreach (var attribute in attributes)
	        {
		        correctMethod |= attribute.HttpMethod == SignalsApiMethod.ANY ||
		                         string.Compare(
			                         attribute.HttpMethod.ToString(),
			                         context.HttpMethod,
			                         StringComparison.InvariantCultureIgnoreCase) == 0;
	        }

	        hasMatchingSignalsApiAttribute = correctMethod;

			// and cache it
	        if (hasMatchingSignalsApiAttribute)
	        {
		        TypeApiAttributeRegistry.TryAdd(type.FullName, true);
	        }

	        return hasMatchingSignalsApiAttribute;
        }
    }
}
