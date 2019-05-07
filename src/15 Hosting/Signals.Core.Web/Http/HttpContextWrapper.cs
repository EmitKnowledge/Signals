using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Signals.Aspects.DI;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Input.Http.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Signals.Core.Web.Http
{
    /// <summary>
    /// Wrapper around real http context
    /// </summary>
    public class HttpContextWrapper : IHttpContextWrapper
    {
        /// <summary>
        /// Http method: GET, POST, DELETE..
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// Url path: /api/process...
        /// </summary>
        public string RawUrl { get; set; }

        /// <summary>
        /// Streams of input files
        /// </summary>
        public IEnumerable<InputFile> Files { get; set; }

        /// <summary>
        /// Request query
        /// </summary>
        public IDictionary<string, IEnumerable<string>> Query { get; set; }

        /// <summary>
        /// Request body
        /// </summary>
        public Lazy<string> Body { get; set; }

        /// <summary>
        /// Headers collection manager
        /// </summary>
        public IHeaderCollection Headers { get; set; }

        /// <summary>
        /// Form collection manager
        /// </summary>
        public Processing.Input.Http.IFormCollection Form { get; set; }

        /// <summary>
        /// Cookies collection manager
        /// </summary>
        public ICookieCollection Cookies { get; set; }

        /// <summary>
        /// Session manager
        /// </summary>
        public ISessionProvider Session { get; set; }

        /// <summary>
        /// Reads body as string
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        private string ExtractBody(Stream inputStream)
        {
            var content = new StreamReader(inputStream).ReadToEnd();
            return content;
        }

#if (NET461)

        /// <summary>
        /// CTOR
        /// </summary>
        public HttpContextWrapper()
        {
            var context = System.Web.HttpContext.Current;
            if (context == null) return;

            Headers = new HeaderCollection(context);
            Cookies = new CookieCollection(context);
            Form = new FormCollection(context);
            Session = new SessionProvider(context);

            var query = QueryHelpers.ParseNullableQuery(context.Request.QueryString.ToString())?
                .Select(x => new KeyValuePair<string, IEnumerable<string>>(x.Key, x.Value.ToArray()));
            Query = query?.ToDictionary(x => x.Key, x => x.Value);

            Body = new Lazy<string>(() => ExtractBody(context.Request.InputStream));
            HttpMethod = context.Request.HttpMethod.ToUpperInvariant();
            Files = context.Request.Files.AllKeys.Select(x => new InputFile
            {
                File = context.Request.Files[x]?.InputStream,
                MimeType = context.Request.Files[x]?.ContentType,
                FormInputName = x,
                FileName = context.Request.Files[x]?.FileName
            });
            RawUrl = context.Request.Url.AbsolutePath;
        }

        /// <summary>
        /// Write response
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        public void PutResponse(HttpResponseMessage httpResponse)
        {
            var context = System.Web.HttpContext.Current;

            if (httpResponse?.Headers?.Any() == true)
                foreach (var header in httpResponse?.Headers)
                    context.Response.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));


            if (httpResponse?.Content?.Headers?.Any() == true)
                foreach (var header in httpResponse?.Content?.Headers)
                    context.Response.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));

            context.Response.StatusCode = (int)httpResponse.StatusCode;

            if (!httpResponse.Content.IsNull())
            {
                var body = httpResponse.Content.ReadAsStreamAsync().Result;
                body.CopyTo(context.Response.OutputStream);
            }
        }

#else

        /// <summary>
        /// CTOR
        /// </summary>
        public HttpContextWrapper(IHttpContextAccessor httpContextAccessor)
        {
            var context = httpContextAccessor.HttpContext;
            if (context == null) return;

            Headers = new HeaderCollection(context);
            Cookies = new CookieCollection(context);
            Form = new FormCollection(context);
            Session = new SessionProvider(context);

            var query = QueryHelpers.ParseNullableQuery(context.Request.QueryString.ToString())?
                .Select(x => new KeyValuePair<string, IEnumerable<string>>(x.Key, x.Value.ToArray()));
            Query = query?.ToDictionary(x => x.Key, x => x.Value);

            Body = new Lazy<string>(() => ExtractBody(context.Request.Body));
            HttpMethod = context.Request.Method.ToUpperInvariant();

            // Form throws exception
            try
            {
                Files = context.Request?.Form?.Files?
                            .Select(x => new InputFile
                            {
                                File = x.OpenReadStream(),
                                FileName = x.FileName,
                                FormInputName = x.Name,
                                MimeType = x.ContentType
                            })
                        ?? new List<InputFile>();
            }
            catch
            {
                Files = new List<InputFile>();
            }

            RawUrl = context.Request.Path.Value;
        }

        /// <summary>
        /// Write response
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        public void PutResponse(HttpResponseMessage httpResponse)
        {
            var httpContextAccessor = SystemBootstrapper.GetInstance<IHttpContextAccessor>();
            var context = httpContextAccessor.HttpContext;

            if (httpResponse?.Headers?.Any() == true)
                foreach (var header in httpResponse?.Headers)
                    context.Response.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));

            if (httpResponse?.Content?.Headers?.Any() == true)
                foreach (var header in httpResponse?.Content?.Headers)
                    context.Response.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));

            context.Response.StatusCode = (int)httpResponse.StatusCode;

            if (!httpResponse.Content.IsNull())
            {
                var body = httpResponse.Content.ReadAsStreamAsync().Result;
                body.CopyToAsync(context.Response.Body).Wait();
            }
        }

#endif

    }
}
