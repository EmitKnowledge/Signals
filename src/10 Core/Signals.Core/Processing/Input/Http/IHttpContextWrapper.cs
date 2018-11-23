using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Signals.Core.Processing.Input.Http
{
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
        IDictionary<string, StringValues> Query { get; set; }

        /// <summary>
        /// Request body
        /// </summary>
        string Body { get; set; }

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
        /// <param name="respose"></param>
        /// <returns></returns>
        void PutResponse(HttpResponseMessage httpResponse);
    }
}
