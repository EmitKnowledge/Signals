using Signals.Core.Common.Serialization;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Results;
using System;
using System.Linq;
using System.Net.Http;

namespace Signals.Core.Web.Helpers
{
    /// <summary>
    /// Helper for processes
    /// </summary>
    public static class ProcessHelper
    {
        /// <summary>
        /// Create serialized http string content
        /// </summary>
        /// <param name="processType"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static StringContent ToHttpContent(this Type processType, VoidResult response)
        {
            var attributes = processType.GetCustomAttributes(typeof(SignalsApiAttribute), false).Cast<SignalsApiAttribute>().ToList();

            foreach (var attribute in attributes)
            {
                if (attribute.ResponseType == SerializationFormat.Xml)
                {
                    return new StringContent(response.SerializeXml(), System.Text.Encoding.UTF8, "application/xml");
                }
                else if (attribute.ResponseType == SerializationFormat.Json)
                {
                    return new StringContent(response.SerializeJson(), System.Text.Encoding.UTF8, "application/json");
                }
            }

            return new StringContent(response.SerializeJson(), System.Text.Encoding.UTF8, "application/json");
        }
    }
}
