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
                    var attribute = type.GetCustomAttributes(typeof(SignalsApiAttribute), false).Cast<SignalsApiAttribute>().SingleOrDefault();
                    var headerAttributes = type
                        .GetCustomAttributes(typeof(ResponseHeaderAttribute), false)
                        .Cast<ResponseHeaderAttribute>()
                        .Concat(headers)
                        .DistinctByEx(x => x.Headers.Keys)
                        .ToList();

                    var contentType = EnumExtensions.GetDescription(SerializationFormat.Json);
                    var httpMethod = SignalsApiMethod.ANY;

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

                    operationItem.Tags = new List<OpenApiTag>
                    {
                        new OpenApiTag
                        {
                            Name = type.Namespace.Split('.').Last()
                        }
                    };
                    operationItem.Summary = instance.Description.IsNullOrEmpty() ? instance.Name : instance.Description;
                    operationItem.Description = instance.Name;
                    operationItem.OperationId = type.Name;

                    // Determine the request based on the process type
                    Type request = null;
                    Type response;
                    var processType = type.BaseType.Name;
                    switch (processType)
                    {
                        case "ProxyApiProcess`5":
                            request = processGenerics[3];
                            break;
                        case "ProxyApiProcess`4":
                            request = processGenerics[1];
                            break;
                        case "ProxyApiProcess`3":
                            break;
                        case "AutoApiProcess`3":
                            request = processGenerics[2];
                            break;
                        case "AutoApiProcess`2":
                            request = processGenerics[1];
                            break;
                        case "AutoApiProcess`1":
                            break;
                        default:
                            request = processGenerics.Length > 1 ? processGenerics.First() : null;
                            break;
                    }

                    // Determine the response based on the process type
                    if (processType.StartsWith("ProxyApiProcess") ||
                        processType.StartsWith("AutoApiProcess"))
                    {
                        var underlyingProcessType = processGenerics.First();
                        var underlyingProcessGenerics = underlyingProcessType.BaseType.GetGenericArguments();
                        response = underlyingProcessGenerics.Any() ? underlyingProcessGenerics.Last() : null;
                    }
                    else
                    {
                        response = processGenerics.Last();
                    }

                    // Determine the payload response
                    if (response != null)
                    {
                        if (response.Name.StartsWith("MethodResult") ||
                            response.Name.StartsWith("ListResult"))
                        {
                            response = response.GetGenericArguments().First();
                        }
                        else if (response.Name.StartsWith("VoidResult"))
                        {
                            response = null;
                        }
                    }

                    var enumTypes = new List<Type>();

                    var requestSchema = Deserialize(request, ref enumTypes);
                    var responseSchema = Deserialize(response, ref enumTypes);

                    var requestPath = string.Empty;
                    if (request != null)
                    {
                        requestPath = $"{request.Namespace.Replace($"{assemblyNamespace}.", string.Empty)}.{request.Name}";
                    }

                    var responsePath = string.Empty;
                    if (response != null)
                    {
                        responsePath = $"{response.Namespace.Replace($"{assemblyNamespace}.", string.Empty)}.{response.Name}";
                    }

                    if (!requestSchema.IsNullOrHasZeroElements())
                    {
                        if (httpMethod == SignalsApiMethod.POST)
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
                                                Reference = request != null
                                                    ? new OpenApiReference
                                                    {
                                                        Id = $"definitions/{requestPath}",
                                                        Type = ReferenceType.Schema,
                                                        ExternalResource = ""
                                                    }
                                                    : null,
                                                Properties = requestSchema
                                            }
                                        }
                                    }
                                }
                            };
                        }
                        else
                        {
                            foreach (var pair in requestSchema)
                            {
                                if (pair.Value.Properties.Any())
                                {
                                    operationItem.Parameters.Add(new OpenApiParameter
                                    {
                                        In = ParameterLocation.Query,
                                        Name = pair.Key,
                                        Reference = request != null
                                            ? new OpenApiReference
                                            {
                                                Id = $"definitions/{requestPath}",
                                                Type = ReferenceType.Schema,
                                                ExternalResource = ""
                                            }
                                            : null,
                                        Schema = pair.Value
                                    });

                                    if (!document.Components.Schemas.ContainsKey(requestPath))
                                    {
                                        document.Components.Schemas.Add($"{requestPath}", new OpenApiSchema { Properties = pair.Value.Properties });
                                    }
                                }
                                else
                                {
                                    operationItem.Parameters.Add(new OpenApiParameter
                                    {
                                        In = ParameterLocation.Query,
                                        Name = pair.Key,
                                        Schema = pair.Value
                                    });
                                }
                            }
                        }
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
                                                Id = $"definitions/{responsePath}",
                                                Type = ReferenceType.Schema,
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

                    if (request != null)
                    {
                        if (!document.Components.Schemas.ContainsKey(requestPath))
                        {
                            document.Components.Schemas.Add(requestPath, new OpenApiSchema { Properties = requestSchema });
                        }
                    }

                    if (response != null)
                    {
                        if (!document.Components.Schemas.ContainsKey(responsePath))
                        {
                            document.Components.Schemas.Add(responsePath, new OpenApiSchema { Properties = responseSchema });
                        }
                    }

                    foreach (var enumType in enumTypes)
                    {
                        if (!document.Components.Schemas.ContainsKey(enumType.Name))
                        {
                            var enums = Enum.GetNames(enumType).Select(x => new OpenApiString(x)).Cast<IOpenApiAny>().ToList();
                            document.Components.Schemas.Add(enumType.Name, new OpenApiSchema { Enum = enums, Type = "string" });
                        }
                    }

                    pathItem.Operations = new Dictionary<OperationType, OpenApiOperation>();

                    foreach (var method in Map(httpMethod))
                    {
                        pathItem.Operations.Add(method, operationItem);
                    }

                    document.Paths.Add(path, pathItem);
                }
            }

            return document;

            Dictionary<string, OpenApiSchema> Deserialize(Type type, ref List<Type> enumTypes)
            {
                if (type == null) return null;

                var result = new Dictionary<string, OpenApiSchema>();
                // Set schema props of the parent property
                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                Stack<OpenApiSchemaNode> toProcess = new Stack<OpenApiSchemaNode>(properties.Select(x => new OpenApiSchemaNode { Property = x }));
                Stack<OpenApiSchema> parentSchemas = new Stack<OpenApiSchema>();

                var MAX_DEPTH = 5;

                // build the object hierarchy
                while (toProcess.Count > 0)
                {
                    var propertyNode = toProcess.Pop();
                    var schema = GetOpenApiSchema(propertyNode.Property, ref enumTypes);
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

            OpenApiSchema GetOpenApiSchema(PropertyInfo property, ref List<Type> enumTypes)
            {
                var schema = new OpenApiSchema
                {
                    Title = property.Name
                };

                // set the property name
                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) &&
                    typeof(string) != property.PropertyType)
                {
                    schema.Type = "array";
                    return schema;
                }
                else if (property.PropertyType.IsEnum)
                {
                    var enums = Enum.GetNames(property.PropertyType).Select(x => new OpenApiString(x)).Cast<IOpenApiAny>().ToList();
                    schema.Enum = enums;
                    schema.Type = property.PropertyType.Name;
                    schema.Format = "int32";
                    schema.Example = enums.FirstOrDefault();
                    schema.Reference = new OpenApiReference
                    {
                        Id = $"definitions/{property.PropertyType.Name}",
                        Type = ReferenceType.Schema,
                        ExternalResource = ""
                    };

                    enumTypes.Add(property.PropertyType);
                    return schema;
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
                        }
                }

                return schema;
            }

            // map http method to open api method
            List<OperationType> Map(SignalsApiMethod method)
            {
                if (method == SignalsApiMethod.ANY)
                    return new List<OperationType>
                    {
                        OperationType.Get,
                        OperationType.Post
                    };
                else if (method == SignalsApiMethod.DELETE)
                    return new List<OperationType> { OperationType.Delete, };
                else if (method == SignalsApiMethod.GET)
                    return new List<OperationType> { OperationType.Get, };
                else if (method == SignalsApiMethod.OPTIONS)
                    return new List<OperationType> { OperationType.Options, };
                else if (method == SignalsApiMethod.PATCH)
                    return new List<OperationType> { OperationType.Patch, };
                else if (method == SignalsApiMethod.POST)
                    return new List<OperationType> { OperationType.Post, };
                else if (method == SignalsApiMethod.PUT)
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

            OpenApiDocument docs;
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

            string docsString;
            string contentType;

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
