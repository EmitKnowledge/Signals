using System;
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
        /// Check if the process type is correct based on the request
        /// </summary>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsCorrectProcessType(Type type, IHttpContextWrapper context)
        {
            return type.GetInterfaces().Contains(typeof(IApiProcess));
        }
    }
}
