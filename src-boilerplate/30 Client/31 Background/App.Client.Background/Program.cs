using App.Service.Configuration;
using App.Service.Controllers.Validation.RuleSpecifications.Base;
using Signals.Aspects.BackgroundProcessing.FluentScheduler;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI.Autofac;
using Signals.Core.Background.Configuration.Bootstrapping;
using Signals.Core.Configuration;
using Signals.Core.Processes.Recurring;
using Signals.Core.Processes.Recurring.Logging;
using System;
using System.IO;
using System.Reflection;

namespace App.Client.Background
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            BusinessConfiguration.UseProvider(new FileConfigurationProvider
            {
                File = @"configs\business.config.json",
                Path = Environment.CurrentDirectory,
                ReloadOnAccess = false
            });

            ApplicationConfiguration.UseProvider(new FileConfigurationProvider
            {
                File = @"configs\application.config.json",
                Path = Environment.CurrentDirectory,
                ReloadOnAccess = false
            });
            
            var regService = new RegistrationService();
            regService.Register<ISyncLogProvider>(new SyncLogProvider());

            var config = new BackgroundApplicationBootstrapConfiguration
            {
                RegistrationService = regService,
                TaskRegistry = new FluentRegistry()
            };

            config.Bootstrap(Assembly.GetExecutingAssembly(), typeof(BaseDomainEntitySpecification<>).Assembly);

            Console.ReadLine();
        }
    }
}
