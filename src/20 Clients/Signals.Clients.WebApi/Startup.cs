using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Signals.Aspects.Auth.NetCore.Extensions;
using Signals.Core.Web.Extensions;

namespace Signals.Clients.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services
                .Configure<CookiePolicyOptions>(opt =>
                {
                    opt.CheckConsentNeeded = context => true;
                    opt.MinimumSameSitePolicy = SameSiteMode.None;
                })
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddCors();

            // Allow large forms data
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
            });

            return services.AddSignals();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseCors(builder => builder
                .WithOrigins(
                    "http://local.menusano.com:8080",
                    "https://local.menusano.com:8080",
                    "http://localhost:8080",
                    "https://local.menusano.com:8099"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseSignalsAuth();
            app.UseSignals();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
