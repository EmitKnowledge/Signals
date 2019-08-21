using Signals.Core.Common.Text;
using Signals.Aspects.DI;
using Signals.Aspects.Benchmarking;
using System;
using System.Collections.Generic;
#if (NET461)
using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
#else
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Signals.Core.Extensions.Benchmarking.WebReport
{
    /// <summary>
    /// Middleware registration extension
    /// </summary>
    public static class Class1
    {

#if (NET461)

        /// <summary>
        /// OWIN middleware registration extension
        /// </summary>
        /// <param name="app"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public static IAppBuilder MapBenchReport(this IAppBuilder app, string route)
        {
            // mediator middleware
            return app.Use(async (IOwinContext context, Func<Task> next) =>
            {
                if (context.Request.Path.Value.ToLowerInvariant().Trim("/") == route.ToLowerInvariant().Trim("/"))
                {
                    await next.Invoke();
                }
                else
                {
                    await next.Invoke();
                }
            });
        }

#else

        /// <summary>
        /// .Net core middleware registration extension
        /// </summary>
        /// <param name="app"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseBenchReport(this IApplicationBuilder app, string route)
        {

            // mediator middleware
            return app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.ToLowerInvariant().Trim("/") == route.ToLowerInvariant().Trim("/"))
                {
                    var benchmarker = SystemBootstrapper.GetInstance<IBenchmarker>();
                    var report = benchmarker.GetEpicReport("", DateTime.Today);
                    
                    ViewResult result = new ViewResult
                    {
                        ViewName = "BenchmarkReport.cshtml",
                        //ViewData = new ViewDataDictionary<Dictionary<Guid, List<BenchmarkEntry>>>()
                    };

                    //result.ViewData.Model = report;

                    var executor = context.RequestServices.GetRequiredService<ViewResultExecutor>();

                    var routeData = context.GetRouteData();
                    var descriptor = new ActionDescriptor();
                    var actionContext = new ActionContext(context, routeData, descriptor);
                    
                    await executor.ExecuteAsync(actionContext, result);
                }
                else
                {
                    await next();
                }
            });
        }

#endif
    }
}
