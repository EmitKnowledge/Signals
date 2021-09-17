using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Signals.Aspects.DI;
using System;
using System.Diagnostics;
using Signals.Core.Web.Execution;
using Signals.Core.Web.Configuration.Bootstrapping;

namespace Signals.Core.Web.Extensions
{
    /// <summary>
    /// Pipeline registration extension
    /// </summary>
    public static class MiddlewareExtensions
    {
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
                if (await mediator.Dispatch() != MiddlewareResult.StopExecutionAndStopMiddlewarePipe)
                    await next();
            });
        }
    }
}