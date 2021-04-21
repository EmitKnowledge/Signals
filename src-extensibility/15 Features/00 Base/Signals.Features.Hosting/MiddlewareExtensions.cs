using Microsoft.AspNetCore.Builder;
using Signals.Features.Base.Configurations.Feature;
using Signals.Features.Base.Configurations.MicroService;

namespace Signals.Features.Hosting
{
    /// <summary>
    /// Pipeline registration extension
    /// </summary>
    public static class MiddlewareExtensions
    {

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
    }
}
