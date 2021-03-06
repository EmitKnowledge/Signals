﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Signals.Aspects.DI;
using Signals.Core.Web.Configuration;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Signals.Core.Web.Execution;
using Signals.Core.Web.Configuration.Bootstrapping;

#if (NET461)

using Microsoft.Owin.Extensions;
using System.Web;
using System.Web.SessionState;
using Owin;
using Microsoft.Owin;

#endif

namespace Signals.Core.Web.Extensions
{
    /// <summary>
    /// Pipeline registration extension
    /// </summary>
    public static class MiddlewareExtensions
    {

#if (NET461)

        /// <summary>
        /// OWIN middleware registration extension
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configurationCallback"></param>
        /// <returns></returns>
        public static IAppBuilder MapSignals(this IAppBuilder app, Action<FluentWebApplicationBootstrapConfiguration> configurationCallback)
        {
            var ass = AppDomain.CurrentDomain.GetAssemblies();
            // get entry assembly
            StackTrace stackTrace = new StackTrace();
	        // TODO: workaround for tests
			var assembly = stackTrace.GetFrame(1).GetMethod().DeclaringType.Assembly;

            // configure web applicaiton
            var configuration = new FluentWebApplicationBootstrapConfiguration();
            configurationCallback(configuration);
            configuration.ScanAssemblies.Add(assembly);
            configuration.Bootstrap(configuration.ScanAssemblies.ToArray());
            
            // register midleware for handling session
            app.Use(async (IOwinContext context, Func<Task> next) =>
            {
                HttpContextBase httpContext = context.Get<HttpContextBase>(typeof(HttpContextBase).FullName);
                httpContext.SetSessionStateBehavior(SessionStateBehavior.Required);
                await next.Invoke();
            });

            // handle session
            app.UseStageMarker(PipelineStage.MapHandler);

            var mediator = SystemBootstrapper.GetInstance<WebMediator>();

            // mediator middleware
            return app.Use(async (IOwinContext context, Func<Task> next) =>
            {
                if (mediator.Dispatch() != MiddlewareResult.StopExecutionAndStopMiddlewarePipe)
                    await next.Invoke();
            });
        }

#else

        /// <summary>
        /// .Net core configuration extension
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configurationCallback"></param>
        /// <returns></returns>
        public static IServiceCollection AddSignals(this IServiceCollection services, Action<FluentWebApplicationBootstrapConfiguration> configurationCallback)
        {
            StackTrace stackTrace = new StackTrace();
            var assembly = stackTrace.GetFrame(1).GetMethod().DeclaringType.Assembly;

            var configuration = new FluentWebApplicationBootstrapConfiguration();
            configurationCallback(configuration);
            configuration.ScanAssemblies.Add(assembly);
            configuration.Bootstrap(configuration.ScanAssemblies.ToArray());

            return services;
        }

        /// <summary>
        /// .Net core middleware registration extension
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSignals(this IApplicationBuilder app)
        {
            var mediator = SystemBootstrapper.GetInstance<WebMediator>();

            // mediator middleware
            return app.Use(async (httpContext, next) =>
            {
                if (mediator.Dispatch() != MiddlewareResult.StopExecutionAndStopMiddlewarePipe)
                    await next();
            });
        }

#endif

    }
}