using Signals.Core.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.Core.Processes.Api
{
    /// <summary>
    /// Process HTTP method
    /// </summary>
    public enum ApiProcessMethod
    {
        ANY,
        GET,
        POST,
        PUT,
        PATCH,
        DELETE,
        OPTIONS
    }

    /// <summary>
    /// Api process attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ApiProcessAttribute : Attribute
    {
        public ApiProcessAttribute()
        {
            HttpMethod = ApiProcessMethod.ANY;
            ExposeApiDocs = true;
            ResponseType = SerializationFormat.Json;
        }

        /// <summary>
        /// Expected HTTP method
        /// </summary>
        public ApiProcessMethod HttpMethod { get; set; }

        /// <summary>
        /// Expose api docs
        /// </summary>
        public bool ExposeApiDocs { get; set; }

        /// <summary>
        /// Expose api docs
        /// </summary>
        public SerializationFormat ResponseType { get; set; }
    }
}
