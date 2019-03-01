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
        /// <summary>
        /// Http method ANY
        /// </summary>
        ANY,

        /// <summary>
        /// Http method GET
        /// </summary>
        GET,

        /// <summary>
        /// Http method POST
        /// </summary>
        POST,

        /// <summary>
        /// Http method PUT
        /// </summary>
        PUT,

        /// <summary>
        /// Http method PATCH
        /// </summary>
        PATCH,

        /// <summary>
        /// Http method DELETE
        /// </summary>
        DELETE,

        /// <summary>
        /// Http method OPTIONS
        /// </summary>
        OPTIONS,

	    /// <summary>
	    /// Http method HEAD
	    /// </summary>
		HEAD
	}

    /// <summary>
    /// Api process attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ApiProcessAttribute : Attribute
    {
        /// <summary>
        /// CTOR
        /// </summary>
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
