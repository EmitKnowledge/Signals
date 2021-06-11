using BenchmarkDotNet.Attributes;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI;
using Signals.Core.Configuration;
using Signals.Core.Configuration.Bootstrapping;
using Signals.Core.Processes;
using Signals.Tests.Core.Performance.Fixture;
using System;
using System.IO;

namespace Signals.Tests.Core.Performance
{
    public class MediatorTests
    {
        /// <summary>
        /// Mediator
        /// </summary>
        private Mediator Mediator;

        public class Config : ApplicationBootstrapConfiguration { }

        protected void Init(IRegistrationService registrationService)
        {
            ApplicationConfiguration.UseProvider(new FileConfigurationProvider
            {
                Path = Path.Combine(AppContext.BaseDirectory, "config"),
                File = "app.json"
            });

            var config = new Config();
            config.RegistrationService = registrationService;
            config.Bootstrap(typeof(MediatorTests).Assembly);
            Mediator = SystemBootstrapper.GetInstance<Mediator>();
        }


        [GlobalSetup(Targets = new[] { "Autofac" })]
        public void AutofacSetup()
        {
            Init(new Aspects.DI.Autofac.RegistrationService());
        }

        [GlobalSetup(Targets = new[] { "SimpleInjector" })]
        public void SimpleInjectorSetup()
        {
            Init(new Aspects.DI.SimpleInjector.RegistrationService(false));
        }

        [GlobalSetup(Targets = new[] { "DotNetCore" })]
        public void DotNetCoreSetup()
        {
            Init(new Aspects.DI.DotNetCore.RegistrationService());
        }

        [Benchmark]
        public void Autofac()
        {
            Benchmark();
        }

        [Benchmark]
        public void SimpleInjector()
        {
            Benchmark();
        }

        [Benchmark]
        public void DotNetCore()
        {
            Benchmark();
        }

        public void Benchmark()
        {
            Mediator.For<SimpleProcess1>().With(new Input { Input1 = 1, Input2 = 1 });
        }
    }
}