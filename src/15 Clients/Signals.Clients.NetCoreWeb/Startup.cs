using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Signals.Aspects.Auditing.AuditNET.Configurations;
using Signals.Aspects.Auth.NetCore.Extensions;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.CommunicationChannels.ServiceBus.Configurations;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.ErrorHandling.Polly;
using Signals.Aspects.Logging.NLog.Configurations;
using Signals.Aspects.Security.Database.Configurations;
using Signals.Aspects.Storage.Database.Configurations;
using Signals.Core.Configuration;
using Signals.Core.Web.Behaviour;
using Signals.Core.Web.Configuration;
using Signals.Core.Web.Extensions;
using System;
using System.Collections.Generic;
using Signals.Aspects.Localization.Database.Configurations;

namespace Signals.Clients.NetCoreWeb
{
    public class Startup
    {
        public Startup(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.AddAuthentication("Cookies")
                    .AddSignalsAuth(opts => { });

            services.AddSession(opts =>
            {
                opts.Cookie = new CookieBuilder();
                opts.Cookie.Name = "Session";
                opts.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                opts.Cookie.MaxAge = TimeSpan.FromMinutes(20);
                opts.Cookie.IsEssential = true;
                opts.Cookie.Expiration = TimeSpan.FromMinutes(20);
                opts.Cookie.HttpOnly = true;
            });

            ApplicationConfiguration
                .UseProvider(new FileConfigurationProvider
                {
                    Path = Environment.CurrentDirectory,
                    File = "appsettings.json",
                    ReloadOnAccess = false
                });

            WebApplicationConfiguration
                .UseProvider(new FileConfigurationProvider
                {
                    Path = @"E:\repos\Emit.Knowledge.Signals\trunk\src\15 Clients\Signals.Clients.MvcWeb",
                    File = "websettings.json",
                    ReloadOnAccess = false
                });

            //var registrationService = new Aspects.DI.Autofac.RegistrationService();
            //registrationService.Builder.Populate(services);
            var registrationService = new Aspects.DI.SimpleInjector.RegistrationService();
            services.AddSignals(config =>
            {
                config.ResponseHeaders = new List<ResponseHeaderAttribute> { new ContentSecurityPolicyAttribute() };
                config.RegistrationService = registrationService;
                config.StrategyBuilder = new StrategyBuilder();
                config.AuditingConfiguration = new FileAuditingConfiguration();
                config.LoggerConfiguration = new DatabaseLoggingConfiguration();
                config.CacheConfiguration = new InMemoryCacheConfiguration();
                config.ChannelConfiguration = new ServiceBusChannelConfiguration
                {
                    ConnectionString = $@"Endpoint=sb://esb-envoice-test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=HzPc16dfugFbBAdW+0vfKtHDJ+sjg8I/GD/cJEPXupk="
                };
                config.LocalizationConfiguration = new DatabaseDataProviderConfiguration("");
                config.StorageConfiguration = new DatabaseStorageConfiguration();
                config.SecurityConfiguration = new DatabaseSecurityConfiguration();
            });

            foreach (var service in registrationService.Builder.GetCurrentRegistrations())
                services.Add(new ServiceDescriptor(service.ServiceType, (provider) => registrationService.Builder.GetInstance(service.ServiceType), ServiceLifetime.Transient));


            //return new AutofacServiceProvider((registrationService.Build() as Aspects.DI.Autofac.ServiceContainer).Container);
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

            app.UseSession();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSignalsAuth();
            app.UseSignals();

            app.UseMvcWithDefaultRoute();
        }
    }
}
