using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Signals.Aspects.DI;
using Signals.Aspects.Caching;
using Signals.Aspects.Caching.Entries;
using Signals.Aspects.Caching.Enums;
using Signals.Core.Processes.Api;
using Signals.Core.Processes.Base;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Configuration;
using Signals.Core.Processing.Execution;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Behaviour;
using Signals.Core.Web.Configuration;
using Signals.Core.Web.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using EnumExtensions = Signals.Core.Common.Instance.EnumExtensions;

namespace Signals.Core.Web.Execution.CustomContentHandlers
{
    /// <summary>
    /// Docs api point handler
    /// </summary>
    public class DocsHandler : ICustomUrlHandler
    {
        /// <summary>
        /// Get api document generated from all api processes
        /// </summary>
        /// <returns></returns>
        private OpenApiDocument GenerateDocs()
        {
            var headers = SystemBootstrapper.GetInstance<List<ResponseHeaderAttribute>>();
            var processRepo = SystemBootstrapper.GetInstance<ProcessRepository>();
            var allApiProcesses = processRepo.OfType<IApiProcess>();

            var document = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Version = ApplicationConfiguration.Instance.ApplicationVersion,
                    Title = ApplicationConfiguration.Instance.ApplicationName,
                },
                Servers = new List<OpenApiServer>
                {
                    new OpenApiServer { Url = WebApplicationConfiguration.Instance.WebUrl }
                },
                Paths = new OpenApiPaths()
            };

            if (allApiProcesses.Any())
            {
                var assemblyNamespace = allApiProcesses.First().Assembly.FullName.Split(',')[0];
                foreach (var type in allApiProcesses)
                {
                    var instance = SystemBootstrapper.GetInstance(type) as IBaseProcess<VoidResult>;
                    var attribute = type.GetCustomAttributes(typeof(ApiProcessAttribute), false).Cast<ApiProcessAttribute>().SingleOrDefault();
                    var headerAttributes = type
                        .GetCustomAttributes(typeof(ResponseHeaderAttribute), false)
                        .Cast<ResponseHeaderAttribute>()
                        .Concat(headers)
                        .DistinctBy(x => x.Name)
                        .ToList();

                    var contentType = EnumExtensions.GetDescription(SerializationFormat.Json);
                    var httpMethod = ApiProcessMethod.ANY;

                    if (!attribute.IsNull())
                    {
                        if (attribute.ExposeApiDocs == false) continue;
                        contentType = EnumExtensions.GetDescription(attribute.ResponseType);
                        httpMethod = attribute.HttpMethod;
                    }

                    var pathItem = new OpenApiPathItem();
                    var operationItem = new OpenApiOperation();

                    var headersDictionary = new Dictionary<string, OpenApiHeader>();

                    foreach (var header in headerAttributes)
                    {
                        headersDictionary.Add(header.Name, new OpenApiHeader
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "string"
                            },
                            Example = new OpenApiString(header.Value)
                        });
                    }

                    operationItem.Description = instance.Description;
                    operationItem.RequestBody = new OpenApiRequestBody
                    {
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            {
                                contentType,
                                new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Properties = Deserialize(type.BaseType.GetGenericArguments().Count() > 1 ? type.BaseType.GetGenericArguments().First() : null)
                                    }
                                }
                            }
                        }
                    };
                    operationItem.Responses = new OpenApiResponses
                    {
                        ["default"] = new OpenApiResponse
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    contentType,
                                    new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Properties = Deserialize(type.BaseType.GetGenericArguments().Last())
                                        }
                                    }
                                }
                            },
                            Headers = headersDictionary
                        }
                    };
                    pathItem.Operations = new Dictionary<OperationType, OpenApiOperation>();

                    foreach (var method in Map(httpMethod))
                    {
                        pathItem.Operations.Add(method, operationItem);
                    }

                    var path = type.FullName.Replace(assemblyNamespace, "").Replace('.', '/');

                    document.Paths.Add(path, pathItem);
                }
            }

            return document;

            Dictionary<string, OpenApiSchema> Deserialize(Type type)
            {
                if (type == null) return null;

                var result = new Dictionary<string, OpenApiSchema>();
                var props = type.GetProperties();

                OpenApiSchema schema = new OpenApiSchema();

                if (typeof(IDisposable).IsAssignableFrom(type))
                {
                    schema.Type = type.Name;
                }
                else if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    schema.Type = type.ToString();
                }
                else
                {
                    if (type.IsPrimitive || type.IsValueType)
                    {
                        schema.Type = type.Name;
                    }
                    else
                    {
                        schema.Properties = props
                            .SelectMany(prop =>
                            {
                                var dic = Deserialize(prop.PropertyType);
                                dic[prop.Name] = dic[prop.PropertyType.Name];
                                dic.Remove(prop.PropertyType.Name);
                                return dic;
                            })
                             .ToLookup(pair => pair.Key, pair => pair.Value)
                             .ToDictionary(group => group.Key, group => group.First());
                    }
                }
                result.Add(type.Name, schema);
                return result;
            };

            // map http method to open api method
            List<OperationType> Map(ApiProcessMethod method)
            {
                if (method == ApiProcessMethod.ANY)
                    return new List<OperationType>
                    {
                        OperationType.Get,
                        OperationType.Post
                    };
                else if (method == ApiProcessMethod.DELETE)
                    return new List<OperationType> { OperationType.Delete, };
                else if (method == ApiProcessMethod.GET)
                    return new List<OperationType> { OperationType.Get, };
                else if (method == ApiProcessMethod.OPTIONS)
                    return new List<OperationType> { OperationType.Options, };
                else if (method == ApiProcessMethod.PATCH)
                    return new List<OperationType> { OperationType.Patch, };
                else if (method == ApiProcessMethod.POST)
                    return new List<OperationType> { OperationType.Post, };
                else if (method == ApiProcessMethod.PUT)
                    return new List<OperationType> { OperationType.Put, };

                return new List<OperationType>();
            }
        }

        /// <summary>
        /// Append docs to http response
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public MiddlewareResult RenderContent(IHttpContextWrapper context)
        {
            if (!context.RawUrl.ToLowerInvariant().Contains("api/docs"))
            {
                return MiddlewareResult.DoNothing;
            }

            var cache = SystemBootstrapper.GetInstance<ICache>();
            var cacheKey = "cache:api/docs";

            OpenApiDocument docs = null;
            var cachedDocs = cache.Get<OpenApiDocument>(cacheKey);

            if (cachedDocs.IsNull())
            {
                docs = GenerateDocs();

                cache.Set(new CacheEntry(cacheKey, docs)
                {
                    ExpirationTime = TimeSpan.FromDays(10000),
                    ExpirationPolicy = CacheExpirationPolicy.Sliding
                });
            }
            else
            {
                docs = cachedDocs;
            }

            string docsString = string.Empty;
            string contentType = string.Empty;

            if (context.RawUrl.ToLowerInvariant().EndsWith(".yaml"))
            {
                docsString = docs.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Yaml);
                contentType = "text/x-yaml";
            }
            else
            {
                docsString = docs.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json);
                contentType = "application/json";
            }

            context.PutResponse(new HttpResponseMessage
            {
                Content = new StringContent(docsString, System.Text.Encoding.UTF8, contentType)
            });

            return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;
        }
    }
}
