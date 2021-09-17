using System;
using System.Collections.Concurrent;
using System.Linq;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Input.Http;

namespace Signals.Core.Web.Execution.Filters
{
	/// <summary>
	/// Process type filter
	/// </summary>
	public class TypeFilter : IFilter
    {
	    /// <summary>
	    /// Cache all api process types that match
	    /// </summary>
	    private static readonly ConcurrentDictionary<string, bool> TypeApiProcessRegistry = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Check if the process type is correct based on the request
        /// </summary>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsCorrectProcessType(Type type, IHttpContextWrapper context)
        {
			// if cached true, return it right away
			TypeApiProcessRegistry.TryGetValue(type.FullName, out var isApiProcessType);
	        if (isApiProcessType) return true;

			// evaluate the condition
			isApiProcessType = type.GetInterfaces().Contains(typeof(IApiProcess));

			// if true cache it
			if (isApiProcessType)
			{
				TypeApiProcessRegistry.TryAdd(type.FullName, true);
			}

			return isApiProcessType;
        }
    }
}
