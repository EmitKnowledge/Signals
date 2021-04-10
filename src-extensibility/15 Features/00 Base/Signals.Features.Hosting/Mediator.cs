using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Reflection;
using Signals.Core.Common.Serialization;
using Signals.Features.Base;
using Signals.Features.Base.Configurations.Feature;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace Signals.Features.Hosting
{
    /// <summary>
    /// Web request mediator
    /// </summary>
    internal class Mediator
    {
        private readonly BaseFeatureConfiguration _configuration;
        private readonly IFeature _server;
        private readonly Dictionary<string, MethodInfo> _serverMethods;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public Mediator(BaseFeatureConfiguration configuration)
        {
            if (configuration.IsNull() || configuration.MicroServiceConfiguration.IsNull())
                throw new ArgumentNullException(nameof(configuration.MicroServiceConfiguration));

            var requiringType = typeof(IFeature);
            var serverType = Directory
                .GetFiles(AppContext.BaseDirectory, "*.dll")
                .Select(Assembly.LoadFrom)
                .SelectMany(assembly => assembly.LoadAllTypesFromAssembly())
                .Where(x => (x.GetInterfaces().Contains(requiringType) || x.IsSubclassOf(requiringType)) && !x.IsInterface && !x.IsAbstract)
                .Where(x => x.GetConstructors().Any(ctor => ctor.GetParameters().Any() && ctor.GetParameters().All(param => param.ParameterType == configuration.GetType())))
                .Distinct()
                .SingleOrDefault();

            _server = Activator.CreateInstance(serverType, configuration) as IFeature;
            _serverMethods = serverType.GetMethods().ToDictionary(x => x.Name, x => x);

            configuration.MicroServiceConfiguration.SendRequestsToMicroService = false;

            _configuration = configuration;
        }


        /// <summary>
        /// Reads body as string
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="inputStream"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        private string ExtractBody(string contentType, Stream inputStream, Dictionary<string, string> form)
        {
            if (contentType?.StartsWith("application/x-www-form-urlencoded") == true
             || contentType?.StartsWith("multipart/form-data") == true)
            {
                return form.SerializeJson();
            }
            else
            {
                var content = new StreamReader(inputStream).ReadToEnd();
                return content;
            }
        }

        /// <summary>
        /// Process http request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        private (bool isSuccess, object response) ProcessRequest(string url, Lazy<string> body, string method, Dictionary<string, string> headers)
        {
            var uri = new Uri($"{_configuration.MicroServiceConfiguration.Url}{url}");
            method = method.ToLowerInvariant();
            
            if (!headers.ContainsKey(nameof(_configuration.MicroServiceConfiguration.ApiKey))
             || !headers.ContainsKey(nameof(_configuration.MicroServiceConfiguration.ApiSecret)))
                return (false, null);

            if (headers[nameof(_configuration.MicroServiceConfiguration.ApiKey)] != _configuration.MicroServiceConfiguration.ApiKey
             || headers[nameof(_configuration.MicroServiceConfiguration.ApiSecret)] != _configuration.MicroServiceConfiguration.ApiSecret)
                return (false, null);

            var path = uri.AbsolutePath.Trim('/');
            var serverMethod = _serverMethods.ContainsKey(path) ? _serverMethods[path] : null;
            if (serverMethod.IsNull())
                return (false, null);

            var queryValues = uri.Query.IsNullOrEmpty() ? null : QueryHelpers.ParseQuery(uri.Query);
            var bodyValue = body.Value.IsNullOrEmpty() ? null : JObject.Parse(body.Value);

            List<object> parameters = new List<object>();

            foreach (var parameter in serverMethod.GetParameters())
            {
                if (queryValues?.ContainsKey(parameter.Name) == true)
                {
                    var parsedParameter = string.Join(", ", queryValues[parameter.Name]).Deserialize(parameter.ParameterType);
                    parameters.Add(parsedParameter);
                }

                if (bodyValue?.ContainsKey(parameter.Name) == true)
                {
                    var parsedParameter = bodyValue.GetValue(parameter.Name).ToObject(parameter.ParameterType);
                    parameters.Add(parsedParameter);
                }
            }

            var result = serverMethod.Invoke(_server, parameters.ToArray());

            if (!serverMethod.ReturnType.IsNull() && serverMethod.ReturnType != typeof(void))
                return (true, result);

            return (true, null);
        }

#if (NET461)

        /// <summary>
        /// Dispatch http request
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public bool Dispatch(System.Web.HttpContext httpContext)
        {
            var headers = new Dictionary<string, string>(httpContext.Request.Headers.Count);

            foreach (var header in httpContext.Request.Headers.AllKeys)
            {
                headers.Add(header, httpContext.Request.Headers[header]);
            }

            var form = new Dictionary<string, string>();
            var requestForm = httpContext.Request?.Form;

            if (!requestForm.IsNull())
            {
                foreach (var key in requestForm.AllKeys)
                {
                    var value = form[key];
                    form.Add(key, value);
                }
            }

            var body = new Lazy<string>(() => ExtractBody(httpContext.Request.ContentType, httpContext.Request.InputStream, form));

            var (isSuccess, response) = ProcessRequest(httpContext.Request.Url.AbsolutePath, body, httpContext.Request.HttpMethod, headers);

            if (isSuccess && !response.IsNull())
                PutResponse(httpContext, new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(response.SerializeJson(), System.Text.Encoding.UTF8, "application/json")
                });

            return isSuccess;
        }

        /// <summary>
        /// Write response
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        public void PutResponse(System.Web.HttpContext httpContext, HttpResponseMessage httpResponse)
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
                body.Close();
            }
        }

#else

        /// <summary>
        /// Dispatch http request
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public bool Dispatch(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            var headers = new Dictionary<string, string>(httpContext.Request.Headers.Count);

            foreach (var header in httpContext.Request.Headers)
            {
                headers.Add(header.Key, header.Value.FirstOrDefault());
            }

            var form = new Dictionary<string, string>();

            if (httpContext.Request.HasFormContentType)
            {
                var requestForm = httpContext.Request?.Form;
                if (requestForm.IsNull())
                {
                    foreach (var key in requestForm.Keys)
                    {
                        var value = requestForm[key];
                        if (value.Count == 1)
                            form.Add(key, value.First());
                        else
                            form.Add(key, value);
                    }
                }
            }

            var body = new Lazy<string>(() => ExtractBody(httpContext.Request.ContentType, httpContext.Request.Body, form));
            
            var (isSuccess, response) = ProcessRequest(httpContext.Request.Path.Value, body, httpContext.Request.Method, headers);

            if (isSuccess && !response.IsNull())
                PutResponse(httpContext, new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(response.SerializeJson(), System.Text.Encoding.UTF8, "application/json")
                });

            return isSuccess;
        }

        /// <summary>
        /// Write response
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        public void PutResponse(Microsoft.AspNetCore.Http.HttpContext httpContext, HttpResponseMessage httpResponse)
        {
            if (httpResponse?.Headers?.Any() == true)
                foreach (var header in httpResponse?.Headers)
                    httpContext.Response.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));

            if (httpResponse?.Content?.Headers?.Any() == true)
                foreach (var header in httpResponse?.Content?.Headers)
                    httpContext.Response.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));

            httpContext.Response.StatusCode = (int)httpResponse.StatusCode;

            if (!httpResponse.Content.IsNull())
            {
                var body = httpResponse.Content.ReadAsStreamAsync().Result;
                body.CopyToAsync(httpContext.Response.Body).Wait();
                body.Close();
            }
        }

#endif

    }
}
