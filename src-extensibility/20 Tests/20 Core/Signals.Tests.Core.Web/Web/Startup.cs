using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Signals.Aspects.Auth.NetCore.Extensions;
using Signals.Core.Common.Instance;
using Signals.Core.Web.Extensions;
using SimpleInjector;
using System;

namespace Signals.Tests.Core.Web.Web
{
    /// <summary>
    /// Statrtup configuration
    /// </summary>
    public class Startup
    {
        public static string ContainerName;
        public static SimpleInjector.Container Container;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Application configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            return services.AddSignals();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (!Container.IsNull())
            {
                app.UseSimpleInjector(Container);
                Container.Verify();
            }

            app.UseDeveloperExceptionPage();

            app.UseSignalsAuth();
            app.UseSignals();

            app.UseMvcWithDefaultRoute();
        }
    }
}