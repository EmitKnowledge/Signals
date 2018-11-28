using App.Domain.Configuration;
using Signals.Aspects.BackgroundProcessing.FluentScheduler;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI.Autofac;
using Signals.Core.Background.Configuration.Bootstrapping;
using Signals.Core.Configuration;

namespace App.Client.Background.Service
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            DomainConfiguration.UseProvider(new FileConfigurationProvider
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

            var config = new BackgroundApplicationBootstrapConfiguration
            {
                RegistrationService = new RegistrationService(),
                TaskRegistry = new FluentRegistry()
            };

            //config.Bootstrap(Assembly.GetExecutingAssembly(), typeof(BaseDomainEntitySpecification<>).Assembly);

            Console.ReadLine();
        }
    }
}