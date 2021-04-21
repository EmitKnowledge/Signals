using System;
using Signals.Aspects.DI;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Execution;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Execution.CustomContentHandlers;
using Signals.Core.Web.Execution.ExecutionHandlers;
using Signals.Core.Web.Execution.ExecutionHandlers.FailedExecution;
using Signals.Core.Web.Execution.FactoryFilters;
using Signals.Core.Web.Execution.Filters;
using System.Collections.Generic;
using System.Linq;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Input.Http.ModelBinding;
using Signals.Core.Processes.Base;
using System.Collections.Concurrent;
using Signals.Core.Common.Reflection;
using Signals.Aspects.DI.Attributes;

namespace Signals.Core.Web.Execution
{
    /// <summary>
    /// Http request mediator
    /// </summary>
    public class WebMediator
    {
        /// <summary>
        /// Custom url handlers
        /// </summary>
        internal List<ICustomUrlHandler> CustomUrlHandlers { get; set; }

        /// <summary>
        /// Process type filters
        /// </summary>
        internal List<IFilter> Filters { get; set; }

        /// <summary>
        /// Process factory creation handler
        /// </summary>
        internal List<IFactoryFilter> FactoryFilters { get; set; }

        /// <summary>
        /// Default model binders
        /// </summary>
        internal Dictionary<SignalsApiMethod, BaseModelBinder> DefaultModelBinders { get; set; }

        /// <summary>
        /// Result handler
        /// </summary>
        internal List<IResultHandler> HeadResultHandlers { get; set; }

        /// <summary>
        /// Result handler
        /// </summary>
        internal List<IResultHandler> OptionsResultHandlers { get; set; }

        /// <summary>
        /// Result handler
        /// </summary>
        internal List<IResultHandler> ResultHandlers { get; set; }

        /// <summary>
        /// Process repository
        /// </summary>
        [Import] internal ProcessRepository ProcessRepository { get; set; }

        /// <summary>
        /// Process factory
        /// </summary>
        [Import] internal IProcessFactory ProcessFactory { get; set; }

        /// <summary>
        /// Process executor
        /// </summary>
        [Import] internal IProcessExecutor ProcessExecutor { get; set; }

        /// <summary>
        /// Url process type cache
        /// </summary>
        private static ConcurrentDictionary<string, Type> UrlTypeCache = new ConcurrentDictionary<string, Type>(8, 10_000);

        /// <summary>
        /// CTOR
        /// </summary>
        public WebMediator()
        {
            // Order is important
            CustomUrlHandlers = new List<ICustomUrlHandler>
            {
                new DocsHandler()
            };

            // order is not important
            Filters = new List<IFilter>
            {
                new TypeFilter(),
                new HttpMethodFilter(),
                new RouteFilter()
            };

            // order is not important
            FactoryFilters = new List<IFactoryFilter> {
                new IsCachedFactoryFilter()
            };

            DefaultModelBinders = new Dictionary<SignalsApiMethod, BaseModelBinder>
            {
                { SignalsApiMethod.GET, new FromQuery() },
                { SignalsApiMethod.POST, new FromBody() },
                { SignalsApiMethod.PUT, new FromBody() },
                { SignalsApiMethod.DELETE, new FromQuery() }
            };

            // order is important, JsonResult is default fallback
            ResultHandlers = new List<IResultHandler>
            {
                new HeaderAdderHandler(),
                new AuthenticationFailResultFilter(),
                new AuthorizationFailResultFilter(),
                new UnmanagedFailResultFilter(),
                new SpecificationFailResultFilter(),
                new CodeSpecificFailResultFilter(),
                new GeneralFailResultFilter(),
                new CacheResultHandler(),
                new FileResultHandler(),
                new XmlResultHandler(),
                new NativeResultHandler(),
                new JsonResultHandler()
            };

            HeadResultHandlers = new List<IResultHandler>
            {
                new HeaderAdderHandler(),
                new ContentTypeHeaderAdderHandler(),
                new EmptyResultHandler()
            };

            OptionsResultHandlers = new List<IResultHandler>
            {
                new AllowHeaderAdderHandler(),
                new EmptyResultHandler()
            };
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <param name="context">For testing</param>
        /// <returns></returns>
        public MiddlewareResult Dispatch(IHttpContextWrapper context = null)
        {
            var httpContext = context ?? SystemBootstrapper.GetInstance<IHttpContextWrapper>();

            // execute custom url handlers
            foreach (var handler in CustomUrlHandlers)
            {
                var result = handler.RenderContent(httpContext);
                // check if url maches
                if (result != MiddlewareResult.DoNothing)
                {
                    return result;
                }
            }

            // get valid process type
            var validType = UrlTypeCache.GetOrAdd(httpContext.RawUrl, (rawUrl) => ProcessRepository.First(type => Filters.All(filter => filter.IsCorrectProcessType(type, httpContext))));
            if (validType.IsNull()) return MiddlewareResult.StopExecutionAndInvokeNextMiddleware;


            // execute head method
            if (httpContext.HttpMethod == SignalsApiMethod.HEAD)
            {
                foreach (var resultHandler in HeadResultHandlers)
                {
                    var result = resultHandler.HandleAfterExecution<IBaseProcess<VoidResult>>(null, validType, VoidResult.Empty, httpContext);
                    if (result != MiddlewareResult.DoNothing)
                    {
                        // flag to stop pipe execution
                        return result;
                    }
                }
            }

            // execute options method
            if (httpContext.HttpMethod == SignalsApiMethod.OPTIONS)
            {
                foreach (var resultHandler in OptionsResultHandlers)
                {
                    var result = resultHandler.HandleAfterExecution<IBaseProcess<VoidResult>>(null, validType, VoidResult.Empty, httpContext);
                    if (result != MiddlewareResult.DoNothing)
                    {
                        // flag to stop pipe execution
                        return result;
                    }
                }
            }

            // create instance
            var process = ProcessFactory.Create<VoidResult>(validType);
            if (process.IsNull()) return MiddlewareResult.StopExecutionAndInvokeNextMiddleware;

            // execute factory filters
            foreach (var factoryFilter in FactoryFilters)
            {
                var result = factoryFilter.IsValidInstance(process, validType, httpContext);
                if (result != MiddlewareResult.DoNothing)
                {
                    return result;
                }
            }

            // determine the parameter binding method
            var parameterBindingAttribute = validType.GetCachedAttributes<BaseModelBinder>().FirstOrDefault();

            // resolve default if not provided
            if (parameterBindingAttribute.IsNull())
            {
                parameterBindingAttribute = DefaultModelBinders.GetValueOrDefault(httpContext.HttpMethod);
            }

            var requestInput = parameterBindingAttribute.Bind(httpContext);

            // execute process
            var response = ProcessExecutor.Execute(process, requestInput);

            // post execution events
            foreach (var resultHandler in ResultHandlers)
            {
                var result = resultHandler.HandleAfterExecution(process, validType, response, httpContext);
                if (result != MiddlewareResult.DoNothing)
                {
                    // flag to stop pipe execution
                    return result;
                }
            }

            // flag to stop pipe execution
            return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;
        }
    }
}