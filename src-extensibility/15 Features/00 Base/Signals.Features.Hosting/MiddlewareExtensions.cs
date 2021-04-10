using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;
using System.Web;
using Signals.Features.Base.Configurations.Feature;
using Signals.Features.Base.Configurations.MicroService;

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
        /// <param name="featureConfiguration"></param>
        /// <param name="microServiceConfiguration"></param>
        /// <returns></returns>
        public static IAppBuilder MapFeature(this IAppBuilder app, BaseFeatureConfiguration featureConfiguration, MicroServiceConfiguration microServiceConfiguration)
        {
            featureConfiguration.MicroServiceConfiguration = microServiceConfiguration;
            var mediator = new Mediator(featureConfiguration);

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
        /// <param name="featureConfiguration"></param>
        /// <param name="microServiceConfiguration"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseFeature(this IApplicationBuilder app, BaseFeatureConfiguration featureConfiguration, MicroServiceConfiguration microServiceConfiguration)
        {
            featureConfiguration.MicroServiceConfiguration = microServiceConfiguration;
            var mediator = new Mediator(featureConfiguration);

            return app.Use(async (httpContext, next) =>
            {
                if (!mediator.Dispatch(httpContext))
                    await next();
            });
        }

#endif

    }
}
