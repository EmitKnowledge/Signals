using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Signals.Aspects.Caching;
using Signals.Aspects.Caching.Entries;
using Signals.Aspects.Caching.Enums;
using Signals.Aspects.DI;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Configuration;
using Signals.Core.Processes.Api;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Execution;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Behaviour;
using Signals.Core.Web.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using EnumExtensions = Signals.Core.Common.Instance.EnumExtensions;

namespace Signals.Core.Web.Execution.CustomContentHandlers
{
    /// <summary>
    /// Docs api point handler
    /// </summary>
    public class DocsHandler : ICustomUrlHandler
    {
        /// <summary>
        /// Represents node visitor state
        /// </summary>
	    class OpenApiSchemaNode
        {
            /// <summary>
            /// Property for processing
            /// </summary>
            public PropertyInfo Property { get; set; }

            /// <summary>
            /// Depth of the property in the class hierarchy
            /// </summary>
            public int Depth { get; set; }
        }

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
                    new OpenApiServer { Url = $"{WebApplicationConfiguration.Instance.WebUrl.TrimEnd('/')}/api/" }
                },
                Paths = new OpenApiPaths(),
                Components = new OpenApiComponents()
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
                        .DistinctBy(x => x.Headers.Keys)
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

                    var path = type.FullName.Replace(assemblyNamespace, "").Replace('.', '/');
                    var processGenerics = type.BaseType.GetGenericArguments();

                    var headersDictionary = new Dictionary<string, OpenApiHeader>();

                    foreach (var header in headerAttributes)
                    {
                        foreach (var headerValue in header.Headers)
                        {
                            headersDictionary.Add(headerValue.Key, new OpenApiHeader
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "string"
                                },
                                Example = new OpenApiString(headerValue.Value)
                            });
                        }
                    }

                    operationItem.Tags = new List<OpenApiTag>();
                    operationItem.Tags.Add(new OpenApiTag
                    {
                        Name = type.Namespace.Split('.').Last()
                    });
                    operationItem.Summary = instance.Description.IsNullOrEmpty() ? instance.Name : instance.Description;
                    operationItem.Description = instance.Name;

                    var request = processGenerics.Count() > 1 ? processGenerics.First() : null;
                    var response = processGenerics.Last();

                    var requestSchema = Deserialize(request);
                    var responseSchema = Deserialize(response);

                    if (httpMethod == ApiProcessMethod.POST)
                    {
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
                                            Reference = processGenerics.Count() > 1 ? new OpenApiReference
                                            {
                                                Id = $"definitions/{type.Name}RequestDto",
                                                Type = ReferenceType.RequestBody,
                                                ExternalResource = ""
                                            } : null,
                                            Properties = requestSchema
                                        }
                                    }
                                }
                            }
                        };
                    }
                    else
                    {
                        operationItem.Parameters.Add(new OpenApiParameter
                        {
                            In = ParameterLocation.Query,
                            Name = "query",
                            Reference = processGenerics.Count() > 1 ? new OpenApiReference
                            {
                                Id = $"definitions/{type.Name}RequestDto",
                                Type = ReferenceType.RequestBody,
                                ExternalResource = ""
                            } : null,
                            Schema = new OpenApiSchema { Properties = responseSchema }
                        });
                    }

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
                                            Reference = new OpenApiReference
                                            {
                                                Id = $"definitions/{type.Name}ResponseDto",
                                                Type = ReferenceType.RequestBody,
                                                ExternalResource = ""
                                            },
                                            Properties = responseSchema
                                        }
                                    }
                                }
                            },
                            Headers = headersDictionary
                        }
                    };

                    document.Components.Schemas.Add($"{type.Name}RequestDto", new OpenApiSchema { Properties = requestSchema });
                    document.Components.Schemas.Add($"{type.Name}ResponseDto", new OpenApiSchema { Properties = responseSchema });

                    pathItem.Operations = new Dictionary<OperationType, OpenApiOperation>();

                    foreach (var method in Map(httpMethod))
                    {
                        pathItem.Operations.Add(method, operationItem);
                    }

                    document.Paths.Add(path, pathItem);
                }
            }

            return document;

            Dictionary<string, OpenApiSchema> Deserialize(Type type)
            {
                if (type == null) return null;

                var result = new Dictionary<string, OpenApiSchema>();
                // set shema props of the parent property
                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                Stack<OpenApiSchemaNode> toProcess = new Stack<OpenApiSchemaNode>(properties.Select(x => new OpenApiSchemaNode { Property = x }));
                Stack<OpenApiSchema> parentSchemas = new Stack<OpenApiSchema>();

                var MAX_DEPTH = 5;

                // build the object hierarchy
                while (toProcess.Count > 0)
                {
                    var propertyNode = toProcess.Pop();
                    var schema = GetOpenApiSchema(propertyNode.Property);
                    var parentSchema = parentSchemas.Count == 0 ? null : parentSchemas.Pop();
                    if (propertyNode.Depth == MAX_DEPTH) continue;

                    if (parentSchema != null)
                    {
                        if (parentSchema.Type == "array")
                        {
                            parentSchema.Items = parentSchema.Items ?? new OpenApiSchema();
                            parentSchema.Items.Properties.Add(schema.Title, schema);
                        }
                        else
                        {
                            parentSchema.Properties.Add(schema.Title, schema);
                        }
                    }
                    else
                    {
                        result.Add(propertyNode.Property.Name, schema);
                    }

                    // process the complex types
                    if (!propertyNode.Property.PropertyType.IsPrimitive &&
                        !propertyNode.Property.PropertyType.IsValueType &&
                        !typeof(IEnumerable).IsAssignableFrom(propertyNode.Property.PropertyType) &&
                        typeof(string) != propertyNode.Property.PropertyType)
                    {
                        properties = propertyNode.Property.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                        foreach (var propertyInfo in properties)
                        {
                            toProcess.Push(new OpenApiSchemaNode { Property = propertyInfo, Depth = propertyNode.Depth + 1 });
                            parentSchemas.Push(schema);
                        }
                    }
                    else if (typeof(IDictionary).IsAssignableFrom(propertyNode.Property.PropertyType))
                    {
                        var argument = propertyNode.Property.PropertyType.GetGenericArguments().LastOrDefault();
                        var value = GetDefaultValue(argument);
                        var obj = new
                        {
                            @string = value
                        };

                        properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

                        foreach (var propertyInfo in properties)
                        {
                            toProcess.Push(new OpenApiSchemaNode { Property = propertyInfo, Depth = propertyNode.Depth + 1 });
                            parentSchemas.Push(schema);
                        }
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(propertyNode.Property.PropertyType) &&
                             typeof(string) != propertyNode.Property.PropertyType)
                    {
                        properties = propertyNode.Property.PropertyType
                                                   .GetGenericArguments()
                                                   .FirstOrDefault()?
                                                   .GetProperties(BindingFlags.Instance | BindingFlags.Public) ?? new PropertyInfo[0];
                        foreach (var propertyInfo in properties)
                        {
                            toProcess.Push(new OpenApiSchemaNode { Property = propertyInfo, Depth = propertyNode.Depth + 1 });
                            parentSchemas.Push(schema);
                        }
                    }
                }

                return result;
            }

            object GetDefaultValue(Type t)
            {
                if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                    return Activator.CreateInstance(t);
                else
                    return null;
            }

            OpenApiSchema GetOpenApiSchema(PropertyInfo property)
            {
                var schema = new OpenApiSchema();
                schema.Title = property.Name;

                // set the property name
                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) &&
                    typeof(string) != property.PropertyType)
                {
                    schema.Type = "array";

                }
                else if (property.PropertyType.IsEnum)
                {
                    var enumDescription = string.Join(" | ", Enum.GetNames(property.PropertyType));
                    schema.Enum = new List<IOpenApiAny> { new OpenApiString(enumDescription) };
                }

                switch (Type.GetTypeCode(property.PropertyType))
                {
                    case TypeCode.Boolean:
                        {
                            schema.Type = "boolean";
                            break;
                        }
                    case TypeCode.Byte:
                        {
                            schema.Type = "string";
                            schema.Format = "byte";
                            break;
                        }
                    case TypeCode.Char:
                        {
                            schema.Type = "string";
                            break;
                        }
                    case TypeCode.DateTime:
                        {
                            schema.Type = "string";
                            schema.Format = "date-time";
                            break;
                        }
                    case TypeCode.Decimal:
                        {
                            schema.Type = "number";
                            schema.Format = "double";
                            break;
                        }
                    case TypeCode.Double:
                        {
                            schema.Type = "number";
                            schema.Format = "double";
                            break;
                        }
                    case TypeCode.Int16:
                        {
                            schema.Type = "integer";
                            schema.Format = "int32";
                            break;
                        }
                    case TypeCode.Int32:
                        {
                            schema.Type = "integer";
                            schema.Format = "int32";
                            break;
                        }
                    case TypeCode.Int64:
                        {
                            schema.Type = "integer";
                            schema.Format = "int64";
                            break;
                        }
                    case TypeCode.SByte:
                        {
                            schema.Type = "string";
                            schema.Format = "byte";
                            break;
                        }
                    case TypeCode.Single:
                        {
                            schema.Type = "number";
                            schema.Format = "float";
                            break;
                        }
                    case TypeCode.String:
                        {
                            schema.Type = "string";
                            break;
                        }
                    case TypeCode.UInt16:
                        {
                            schema.Type = "integer";
                            schema.Format = "int32";
                            break;
                        }
                    case TypeCode.UInt32:
                        {
                            schema.Type = "integer";
                            schema.Format = "int32";
                            break;
                        }
                    case TypeCode.UInt64:
                        {
                            schema.Type = "integer";
                            schema.Format = "int64";
                            break;
                        };
                }

                return schema;
            }

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
            if (!context.RawUrl.ToLowerInvariant().Contains("api/spec"))
            {
                return MiddlewareResult.DoNothing;
            }

            var cache = SystemBootstrapper.GetInstance<ICache>();
            var cacheKey = "cache:api/spec";

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
