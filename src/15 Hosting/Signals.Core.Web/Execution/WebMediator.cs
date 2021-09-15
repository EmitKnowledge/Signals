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
using System.Threading.Tasks;

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
				{ SignalsApiMethod.OPTIONS,  new FromQuery() },
		        { SignalsApiMethod.GET,  new FromQuery() },
		        { SignalsApiMethod.HEAD,  new FromHeader() },
		        { SignalsApiMethod.POST,  new FromBody() },
		        { SignalsApiMethod.PUT,  new FromBody() },
		        { SignalsApiMethod.DELETE, new FromQuery() }
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
                var validType = ProcessRepository.All(type => Filters.All(filter => filter.IsCorrectProcessType(type, httpContext))).FirstOrDefault();
                if (validType.IsNull()) return MiddlewareResult.StopExecutionAndInvokeNextMiddleware;

                // create instance
                var process = ProcessFactory.Create<VoidResult>(validType);
                if (process.IsNull()) return MiddlewareResult.StopExecutionAndInvokeNextMiddleware;

                // execute factory filters
                foreach (var createEvent in FactoryFilters)
                {
                    var result = createEvent.IsValidInstance(process, validType, httpContext);
                    if (result != MiddlewareResult.DoNothing)
                    {
                        return result;
                    }
                }

                var method = EnumExtensions.FromString<SignalsApiMethod>(httpContext.HttpMethod?.ToUpper());

                // determine the parameter binding method
                var parameterBindingAttribute = validType?
                    .GetCustomAttributes(typeof(SignalsParameterBindingAttribute), false)
                    .Cast<SignalsParameterBindingAttribute>()
                    .FirstOrDefault();

                // resolve default if not provided
                if (parameterBindingAttribute.IsNull())
                {
                    DefaultModelBinders.TryGetValue(method, out var modelBinder);
                    parameterBindingAttribute = new SignalsParameterBindingAttribute(modelBinder);
                }

                var requestInput = parameterBindingAttribute.Binder.Bind(httpContext);

                // execute process
                var response = new VoidResult();

                // decide if we need to execute
                switch (method)
                {

                    case SignalsApiMethod.OPTIONS:
                    case SignalsApiMethod.HEAD:
                        break;
                    default:
                        response = ProcessExecutor.Execute(process, requestInput);
                        break;
                }

                // post execution events
                foreach (var executeEvent in ResultHandlers)
                {
                    var result = executeEvent.HandleAfterExecution(process, validType, response, httpContext);
                    if (result != MiddlewareResult.DoNothing)
                    {
                        // flag to stop pipe execution
                        return result;
                    }
                }

                // flag to stop pipe execution
                return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;
            });
        }
    }
}