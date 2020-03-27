﻿using System;
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
		internal Dictionary<ApiProcessMethod, Type> DefaultModelBinders { get; set; }

        /// <summary>
        /// Result handler
        /// </summary>
        internal List<IResultHandler> ResultHandlers { get; set; }

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

	        DefaultModelBinders = new Dictionary<ApiProcessMethod, Type>
	        {
				{ ApiProcessMethod.OPTIONS,  typeof(FromQuery) },
		        { ApiProcessMethod.GET,  typeof(FromQuery) },
		        { ApiProcessMethod.HEAD,  typeof(FromHeader) },
		        { ApiProcessMethod.POST,  typeof(FromBody) },
		        { ApiProcessMethod.PUT,  typeof(FromBody) },
		        { ApiProcessMethod.DELETE, typeof(FromQuery) }
			};

			// Order is important, JsonResult is default fallback
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
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <returns></returns>
        public MiddlewareResult Dispatch()
        {
            var httpContext = SystemBootstrapper.GetInstance<IHttpContextWrapper>();
	        var method = EnumExtensions.FromString<ApiProcessMethod>(httpContext.HttpMethod?.ToUpper());

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

            // get process repo instance
            var processRepo = SystemBootstrapper.GetInstance<ProcessRepository>();

            // get valid process type
            var validType = processRepo.All(type => Filters.All(filter => filter.IsCorrectProcessType(type, httpContext))).FirstOrDefault();

            // flag to continue pipe execution
            if (validType.IsNull()) return MiddlewareResult.StopExecutionAndInvokeNextMiddleware;

            // get factory and executor instances
            var factory = SystemBootstrapper.GetInstance<IProcessFactory>();
            var executor = SystemBootstrapper.GetInstance<IProcessExecutor>();

            // create instance
            var process = factory.Create<VoidResult>(validType);

            // flag to continue pipe execution
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
	        
			var param = parameterBindingAttribute.Binder.Bind(httpContext);

			// execute process
			var response = new VoidResult();

			// decide if we need to execute
	        switch (method)
	        {

				case ApiProcessMethod.OPTIONS:
				case ApiProcessMethod.HEAD:
					break;
				default:
					response = executor.Execute(process, param);
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
        }
    }
}