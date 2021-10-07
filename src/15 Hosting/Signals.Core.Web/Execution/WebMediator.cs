using System;
using System.Collections.Concurrent;
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
using System.Threading.Tasks;
using Signals.Core.Common.Reflection;
using Signals.Core.Processes.Base;

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
		/// Cache all process types that match
		/// </summary>
		private static readonly ConcurrentDictionary<string, BaseModelBinder> TypeDefaultModelBindersRegistry = new ConcurrentDictionary<string, BaseModelBinder>();

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
        internal ProcessRepository ProcessRepository { get; set; }

        /// <summary>
        /// Process factory
        /// </summary>
        internal IProcessFactory ProcessFactory { get; set; }

        /// <summary>
        /// Process executor
        /// </summary>
        internal IProcessExecutor ProcessExecutor { get; set; }

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
	            new RouteFilter(),
                new TypeFilter(),
                new HttpMethodFilter()
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

            // order is important, JsonResult is default fallback
            ResultHandlers = new List<IResultHandler> {
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
            
            // get process repo instance
            ProcessRepository = SystemBootstrapper.GetInstance<ProcessRepository>();
            ProcessFactory = SystemBootstrapper.GetInstance<IProcessFactory>();
            ProcessExecutor = SystemBootstrapper.GetInstance<IProcessExecutor>();
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <param name="context">For testing</param>
        /// <returns></returns>
        public async Task<MiddlewareResult> Dispatch(IHttpContextWrapper context = null)
        {
            return await Task.Run(() => {
                var httpContext = context ?? SystemBootstrapper.GetInstance<IHttpContextWrapper>();
                
                // execute custom url handlers
                var result = ExecuteCustomUrlHandlers(httpContext);
                if (result != MiddlewareResult.DoNothing) return result;

                // get valid process type
                var validType = ExecuteFiltersToDiscoverMatchingProcessType(httpContext);
                if (validType.IsNull()) return MiddlewareResult.StopExecutionAndInvokeNextMiddleware;

                // try execute HEAD request if there is a HTTP method match
                result = ExecuteHeadMethod(httpContext, validType);
                if (result != MiddlewareResult.DoNothing) return result;

                // try execute OPTIONS request if there is a HTTP method match
                result = ExecuteOptionsMethod(httpContext, validType);
                if (result != MiddlewareResult.DoNothing) return result;

                // create instance
                var process = CreateInstanceFor(httpContext, validType);
                if (process.IsNull()) return MiddlewareResult.StopExecutionAndInvokeNextMiddleware;

                // execute request parsing and mapping to Signals HTTP context
                httpContext.Wrap();

                result = ExecuteFactoryFilters(httpContext, validType, process);
                if (result != MiddlewareResult.DoNothing) return result;

                // execute process
                var response = ExecuteGetOrPostMethod(httpContext, validType, process);
                result = ExecuteResultHandlers(httpContext, validType, process, response);
                return result;
            });
        }

        /// <summary>
        /// Execute custom url handlers 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private MiddlewareResult ExecuteCustomUrlHandlers(IHttpContextWrapper httpContext)
        {
	        // execute custom url handlers
	        foreach (var customUrlHandler in CustomUrlHandlers)
	        {
		        var result = customUrlHandler.RenderContent(httpContext);
		        // check if url matches
		        if (result != MiddlewareResult.DoNothing)
		        {
			        this.D($"Custom Url handler: {customUrlHandler.GetType().FullName} resulted in halt for -> URL: {httpContext.RawUrl}.");
			        return result;
		        }
	        }
	        this.D($"Executed Custom Url Handlers for -> URL: {httpContext.RawUrl}.");
	        return MiddlewareResult.DoNothing;
        }

        /// <summary>
        /// Find the valid process type that match all of the available filters
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private Type ExecuteFiltersToDiscoverMatchingProcessType(IHttpContextWrapper httpContext)
        {
	        // get valid process type
	        var validType = UrlTypeCache.GetOrAdd(httpContext.RawUrl, (rawUrl) => ProcessRepository.First(type => Filters.All(filter => filter.IsCorrectProcessType(type, httpContext))));
	        if (validType.IsNull())
	        {
		        this.D($"Process Type not found for -> URL: {httpContext.RawUrl}.");
                return null;
	        }
	        this.D($"Process Type: {validType.FullName} found for -> URL: {httpContext.RawUrl}.");
	        return validType;
        }

        /// <summary>
        /// Execute head request
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="processType"></param>
        /// <returns></returns>
        private MiddlewareResult ExecuteHeadMethod(IHttpContextWrapper httpContext, Type processType)
        {
            // execute head method
            if (httpContext.HttpMethod != SignalsApiMethod.HEAD) return MiddlewareResult.DoNothing;

            foreach (var resultHandler in HeadResultHandlers)
            {
	            var result = resultHandler.HandleAfterExecution<IBaseProcess<VoidResult>>(null, processType, VoidResult.Default(), httpContext);
	            if (result != MiddlewareResult.DoNothing)
	            {
                    // flag to stop pipe execution
                    this.D($"Head Result handler: {resultHandler.GetType().FullName} resulted in halt for -> URL: {httpContext.RawUrl}.");
                    return result;
	            }
            }

            return MiddlewareResult.DoNothing;
        }

        /// <summary>
        /// Execute head request
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="processType"></param>
        /// <returns></returns>
        private MiddlewareResult ExecuteOptionsMethod(IHttpContextWrapper httpContext, Type processType)
        {
	        // execute head method
	        if (httpContext.HttpMethod != SignalsApiMethod.OPTIONS) return MiddlewareResult.DoNothing;

	        foreach (var resultHandler in OptionsResultHandlers)
	        {
		        var result = resultHandler.HandleAfterExecution<IBaseProcess<VoidResult>>(null, processType, VoidResult.Default(), httpContext);
		        if (result != MiddlewareResult.DoNothing)
		        {
                    // flag to stop pipe execution
                    this.D($"Options Result handler: {resultHandler.GetType().FullName} resulted in halt for -> URL: {httpContext.RawUrl}.");
                    return result;
		        }
	        }
	        
	        return MiddlewareResult.DoNothing;
        }

        /// <summary>
        /// Execute head request
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="processType"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        private VoidResult ExecuteGetOrPostMethod(IHttpContextWrapper httpContext, Type processType, IBaseProcess<VoidResult> process)
        {
	        // execute head method
	        if (httpContext.HttpMethod != SignalsApiMethod.GET && httpContext.HttpMethod != SignalsApiMethod.POST) return VoidResult.Default();

	        // if cached true, return it right away
	        var parameterBinding = TypeDefaultModelBindersRegistry.GetOrAdd(processType.FullName, _ =>
	        {
		        // determine the parameter binding method
		        var parameterBindingAttribute = processType?.GetCachedAttributes<SignalsParameterBindingAttribute>().FirstOrDefault();

                // resolve default if not provided
                if (parameterBindingAttribute.IsNull())
		        {
			        DefaultModelBinders.TryGetValue(httpContext.HttpMethod, out var modelBinder);
			        this.D($"Model binder not found for -> Process Type: {processType.FullName} for -> URL: {httpContext.RawUrl}. Assigning default model binder.");
			        return modelBinder;
		        }
		        else
		        {
			        this.D($"Model binder: {parameterBindingAttribute?.Binder?.GetType().FullName} found for Process Type: {processType.FullName} for -> URL: {httpContext.RawUrl}.");
		        }

		        return parameterBindingAttribute?.Binder;
	        });

	        var requestInput = parameterBinding.Bind(httpContext);
	        this.D($"Request input transformed for -> Process Type: {processType.FullName} for -> URL: {httpContext.RawUrl}.");

	        var response = ProcessExecutor.Execute(process, requestInput);
	        this.D($"Process executed for -> Process Type: {processType.FullName} for -> URL: {httpContext.RawUrl}.");

            return response;
        }

        /// <summary>
        /// Create an instance for the provided process type
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="processType"></param>
        /// <returns></returns>
        private IBaseProcess<VoidResult> CreateInstanceFor(IHttpContextWrapper httpContext, Type processType)
        {
	        // create instance
	        var process = ProcessFactory.Create<VoidResult>(processType);
	        if (process.IsNull())
	        {
		        this.D($"Failed to create instance for -> Process Type: {processType.FullName} for -> URL: {httpContext.RawUrl}.");
		        return null;
	        }
	        this.D($"Instance created for Process Type: {processType.FullName} found for -> URL: {httpContext.RawUrl}.");
	        return process;
        }

        /// <summary>
        /// Execute default factory filters
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="processType"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        private MiddlewareResult ExecuteFactoryFilters(IHttpContextWrapper httpContext, Type processType, IBaseProcess<VoidResult> process)
        {
	        // execute factory filters
	        foreach (var factoryFilter in FactoryFilters)
	        {
		        var result = factoryFilter.IsValidInstance(process, processType, httpContext);
		        if (result != MiddlewareResult.DoNothing)
		        {
			        this.D($"Factory filter: {factoryFilter.GetType().FullName} resulted in halt for -> Process Type: {processType.FullName} for -> URL: {httpContext.RawUrl}.");
			        return result;
		        }
	        }
	        this.D($"Factory filters executed for -> Process Type: {processType.FullName} for -> URL: {httpContext.RawUrl}.");
	        return MiddlewareResult.DoNothing;
        }

        /// <summary>
        /// Execute the default result handlers
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="processType"></param>
        /// <param name="process"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private MiddlewareResult ExecuteResultHandlers(IHttpContextWrapper httpContext, Type processType, IBaseProcess<VoidResult> process, VoidResult response)
        {
	        // post execution events
	        foreach (var resultHandler in ResultHandlers)
	        {
		        var result = resultHandler.HandleAfterExecution(process, processType, response, httpContext);
		        if (result != MiddlewareResult.DoNothing)
		        {
			        return result;
		        }
	        }
	        this.D($"Result handlers executed for -> Process Type: {processType.FullName} for -> URL: {httpContext.RawUrl}.");
	        return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;
        }
    }
}