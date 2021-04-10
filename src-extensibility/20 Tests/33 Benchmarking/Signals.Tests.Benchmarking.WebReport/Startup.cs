using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Signals.Aspects.Benchmarking.Database.Configurations;
using Signals.Aspects.DI.Autofac;
using Signals.Core.Configuration;
using Signals.Core.Extensions.Benchmarking.WebReport;
using Signals.Core.Web.Configuration;
using Signals.Core.Web.Extensions;

namespace Signals.Tests.Benchmarking.WebReport
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var registrationService = new RegistrationService();


            Signals.Aspects.Configuration.File.FileConfigurationProvider ProviderForFile(string name) => new Signals.Aspects.Configuration.File.FileConfigurationProvider
            {
                File = name,
                Path = Path.Combine(AppContext.BaseDirectory, $"configs"),
                ReloadOnAccess = false
            };

            WebApplicationConfiguration.UseProvider(ProviderForFile("web.application.config.json"));
            ApplicationConfiguration.UseProvider(ProviderForFile("application.config.json"));

            var assemblies = Directory.GetFiles(AppContext.BaseDirectory, "*.dll").Select(Assembly.LoadFrom).ToList();

            registrationService.Builder.Populate(services);

            services.AddSignals(config => {
                config.ScanAssemblies = assemblies;
                config.RegistrationService = registrationService;
                config.BenchmarkingConfiguration = new DatabaseBenchmarkingConfiguration
                {
                    ConnectionString = "server=ohd-md-test.database.windows.net;Database=ohd.db;User Id=ohd.usr;Password = jzpJYcM9SCgSMKsBwLdj",
                    IsEnabled = true
                };
            });

            return new AutofacServiceProvider(registrationService.ServiceContainer.Container);
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseBenchReport("/report");
            app.UseSignals();
            
            app.UseMvc();
        }
    }
}
