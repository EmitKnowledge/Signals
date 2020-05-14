using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;
using System.Web;
using Signals.Features.Base.Configurations.Feature;

#if (NET461)
using Microsoft.Owin;
using Owin;
#endif

namespace Signals.Features.Hosting
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
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IAppBuilder MapFeature(this IAppBuilder app, IFeatureConfiguration configuration)
        {
            var mediator = new Mediator(configuration);

            return app.Use(async (IOwinContext context, Func<Task> next) =>
            {
                if (!mediator.Dispatch(HttpContext.Current))
                    await next.Invoke();
            });
        }

#else
        /// <summary>
        /// .Net core middleware registration extension
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseFeature(this IApplicationBuilder app, IFeatureConfiguration configuration)
        {
            var mediator = new Mediator(configuration);

            return app.Use(async (httpContext, next) =>
            {
                if (!mediator.Dispatch(httpContext))
                    await next();
            });
        }

#endif

    }
}
