using Signals.Core.Common.Serialization;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Results;
using System;
using System.Linq;
using System.Net.Http;
using Signals.Core.Common.Reflection;

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
            var attributes = processType.GetCachedAttributes<SignalsApiAttribute>();

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

        /// <summary>
        /// Create serialized http string content
        /// </summary>
        /// <param name="processType"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string HttpContentType(this Type processType, VoidResult response)
        {
            if (response is FileResult fileResponse)
            {
                return fileResponse.MimeType;
            }

            var attributes = processType.GetCachedAttributes<SignalsApiAttribute>();

            foreach (var attribute in attributes)
            {
                if (attribute.ResponseType == SerializationFormat.Xml)
                {
                    return "application/xml";
                }
                else if (attribute.ResponseType == SerializationFormat.Json)
                {
                    return "application/json";
                }
            }

            return "text/plain";
        }

        /// <summary>
        /// Convert result to status code
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static System.Net.HttpStatusCode ToStatusCode(this VoidResult response)
        {
            if (response.IsSystemFault) return System.Net.HttpStatusCode.InternalServerError;
            if (response.IsFaulted) return System.Net.HttpStatusCode.BadRequest;
            return System.Net.HttpStatusCode.OK;
        }
    }
}