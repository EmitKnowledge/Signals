using App.Domain.Configuration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI.Autofac;
using Signals.Core.Configuration;
using Signals.Core.Web.Configuration;
using Signals.Core.Web.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace App.Client.Web
{
    /// <summary>
    /// Signals bootstrapping class
    /// </summary>
    public static class SignalsStartup
    {
        /// <summary>
        /// Add signals aspects
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceProvider AddSignals(this IServiceCollection services)
        {
            // integrate autofac into mvc.core
            var registrationService = new RegistrationService();
            registrationService.Builder.Populate(services);

            services
                .AddConfiguration()
                .AddSignals(config =>
                {
                    //config.ScanAssemblies = new List<Assembly> { Assembly.GetExecutingAssembly(), typeof(BaseDomainEntitySpecification<>).Assembly };
                    config.RegistrationService = registrationService;
                });

            // wrap autofac container
            return new AutofacServiceProvider(registrationService.ServiceContainer.Container);
        }

        /// <summary>
        /// Load configuration from files
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddConfiguration(this IServiceCollection services)
        {
            FileConfigurationProvider ProviderForFile(string name) => new FileConfigurationProvider
            {
                File = name,
                Path = Path.Combine(Environment.CurrentDirectory, "configs"),
                ReloadOnAccess = false
            };

            DomainConfiguration.UseProvider(ProviderForFile("business.config.json"));
            ApplicationConfiguration.UseProvider(ProviderForFile("application.config.json"));
            WebApplicationConfiguration.UseProvider(ProviderForFile("web.application.config.json"));

            return services;
        }
    }
}