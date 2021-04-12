using Signals.Core.Common.Serialization;
using System;
using System.Net.Http;

namespace Signals.Core.Processes.Api
{
    /// <summary>
    /// Process HTTP method
    /// </summary>
    public enum SignalsApiMethod
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
    public class SignalsApiAttribute : Attribute
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public SignalsApiAttribute()
        {
            HttpMethod = SignalsApiMethod.ANY;
            ExposeApiDocs = true;
            ResponseType = SerializationFormat.Json;
        }

        /// <summary>
        /// Expected HTTP method
        /// </summary>
        public SignalsApiMethod HttpMethod { get; set; }

        /// <summary>
        /// Expose api docs
        /// </summary>
        public bool ExposeApiDocs { get; set; }

        /// <summary>
        /// Expose api docs
        /// </summary>
        public SerializationFormat ResponseType { get; set; }

        /// <summary>
        /// Indicates if the api process should return a native response
        /// </summary>
        public bool NativeResponse { get; set; }
    }
}
