using App.Service.Controllers.Validation.RuleSpecifications.Base;
using Signals.Aspects.BackgroundProcessing.FluentScheduler;
using Signals.Aspects.DI.Autofac;
using Signals.Core.Background.Configuration.Bootstrapping;
using System;
using System.IO;
using System.Reflection;

namespace App.Client.BackgroundServiceWorker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            var config = new BackgroundApplicationBootstrapConfiguration
            {
                RegistrationService = new RegistrationService(),
                TaskRegistry = new FluentRegistry()
            };

            config.Bootstrap(Assembly.GetExecutingAssembly(), typeof(BaseDomainEntitySpecification<>).Assembly);

            Console.ReadLine();
        }
    }
}
