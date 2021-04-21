using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Http;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI;
using Signals.Core.Configuration;
using Signals.Core.Processes.Api;
using Signals.Core.Web.Configuration;
using Signals.Core.Web.Configuration.Bootstrapping;
using Signals.Core.Web.Execution;
using Signals.Core.Web.Http;
using Signals.Tests.Core.Performance.Fixture;
using System;
using System.IO;
using System.Text;

namespace Signals.Tests.Core.Performance
{
    public class WebMediatorTests
    {
        /// <summary>
        /// Web mediator
        /// </summary>
        private WebMediator WebMediator;

        public class Config : WebApplicationBootstrapConfiguration { }

        protected void Init(IRegistrationService registrationService)
        {
            ApplicationConfiguration.UseProvider(new FileConfigurationProvider
            {
                Path = Path.Combine(AppContext.BaseDirectory, "config"),
                File = "app.json"
            });
            WebApplicationConfiguration.UseProvider(new FileConfigurationProvider
            {
                Path = Path.Combine(AppContext.BaseDirectory, "config"),
                File = "webapp.json"
            });

            var config = new Config();
            config.RegistrationService = registrationService;
            config.Bootstrap(typeof(MediatorTests).Assembly);

            WebMediator = SystemBootstrapper.GetInstance<WebMediator>();

            httpContextAccessor = Request();
            httpContextWrapper = RequestWrapping();
        }


        [GlobalSetup(Targets = new[] { "Autofac", "AutofacObject", "AutofacString" })]
        public void AutofacSetup()
        {
            Init(new Aspects.DI.Autofac.RegistrationService());
        }

        [GlobalSetup(Targets = new[] { "SimpleInjector", "SimpleInjectorObject", "SimpleInjectorString" })]
        public void SimpleInjectorSetup()
        {
            Init(new Aspects.DI.SimpleInjector.RegistrationService(false));
        }

        [GlobalSetup(Targets = new[] { "DotNetCore", "DotNetCoreObject", "DotNetCoreString" })]
        public void DotNetCoreSetup()
        {
            Init(new Aspects.DI.DotNetCore.RegistrationService());
        }


        private HttpContextAccessor httpContextAccessor;
        private HttpContextWrapper httpContextWrapper;

        public HttpContextAccessor Request()
        {
            var path = "/api/Fixture/WebProcess";
            var method = SignalsApiMethod.POST;
            var queryString = "";

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Response.Headers.Clear();
            context.Request.Path = path;
            context.Request.Method = method.ToString();

            context.Request.QueryString = QueryString.FromUriComponent(queryString);
            context.Request.Body = new MemoryStream(Encoding.Default.GetBytes("{Input2:1}"));

            return new HttpContextAccessor { HttpContext = context };
        }

        public HttpContextWrapper RequestWrapping()
        {
            return new HttpContextWrapper(Request());
        }

        [Benchmark] public void Autofac() => Benchmark();
        [Benchmark] public void SimpleInjector() => Benchmark();
        [Benchmark] public void DotNetCore() => Benchmark();

        public void Benchmark()
        {
            httpContextAccessor.HttpContext.Response.Body = new MemoryStream();
            httpContextAccessor.HttpContext.Response.Headers.Clear();
            httpContextWrapper.Headers.RemoveAllFromResponse();

            WebMediator.Dispatch(httpContextWrapper);
        }
    }
}