﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Input.Http.Models;

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
        SignalsApiMethod HttpMethod { get; set; }

        /// <summary>
        /// Url path: /api/process...
        /// </summary>
        string RawUrl { get; set; }

        /// <summary>
        /// Streams of input files
        /// </summary>
        IEnumerable<InputFile> Files { get; set; }

        /// <summary>
        /// Request query
        /// </summary>
        IDictionary<string, object> Query { get; set; }

        /// <summary>
        /// Request body
        /// </summary>
        string Body { get; set; }

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
        /// Execute the wrapping to process the request
        /// </summary>
        void Wrap();

        /// <summary>
        /// Write response
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        void PutResponse(HttpResponseMessage httpResponse);
    }
}
