using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Signals.Core.Processing.Input.Http
{
    /// <summary>
    /// Wrapper for MVC http context
    /// </summary>
    public interface IHttpContextWrapper
    {
        /// <summary>
        /// Http method: GET, POST, DELETE..
        /// </summary>
        string HttpMethod { get; set; }

        /// <summary>
        /// Url path: /api/process...
        /// </summary>
        string RawUrl { get; set; }

        /// <summary>
        /// Streams of input files
        /// </summary>
        IEnumerable<Stream> Files { get; set; }

        /// <summary>
        /// Request query
        /// </summary>
        IDictionary<string, IEnumerable<string>> Query { get; set; }

        /// <summary>
        /// Request body
        /// </summary>
        Lazy<string> Body { get; set; }

        /// <summary>
        /// Form collection manager
        /// </summary>
        IFormCollection Form { get; set; }

        /// <summary>
        /// Headers collection manager
        /// </summary>
        IHeaderCollection Headers { get; set; }

        /// <summary>
        /// Cookies collection manager
        /// </summary>
        ICookieCollection Cookies { get; set; }

        /// <summary>
        /// Session manager
        /// </summary>
        ISessionProvider Session { get; set; }


        /// <summary>
        /// Write response
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        void PutResponse(HttpResponseMessage httpResponse);
    }
}
