using System;
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
        /// Check if the process type is correct based on the request
        /// </summary>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsCorrectProcessType(Type type, IHttpContextWrapper context)
        {
            var attributes = type.GetCustomAttributes(typeof(SignalsApiAttribute), false).Cast<SignalsApiAttribute>().ToList();
            if (!attributes.Any()) return true;

            var correctMethod = false;
            foreach (var attribute in attributes)
            {
                correctMethod |= attribute.HttpMethod == SignalsApiMethod.ANY || attribute.HttpMethod.ToString().ToUpperInvariant() == context.HttpMethod.ToUpperInvariant();
            }

            return correctMethod;
        }
    }
}
