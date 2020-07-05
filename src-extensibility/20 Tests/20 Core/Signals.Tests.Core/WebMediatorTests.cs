using Microsoft.AspNetCore.Http;
using Moq;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI;
using Signals.Core.Configuration;
using Signals.Core.Configuration.Bootstrapping;
using Signals.Core.Web.Configuration;
using Signals.Core.Web.Configuration.Bootstrapping;
using Signals.Core.Web.Execution;
using Signals.Core.Web.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Signals.Tests.Core
{
    public class WebMediatorTests
    {
        /// <summary>
        /// Web mediator
        /// </summary>
        private WebMediator WebMediator => SystemBootstrapper.GetInstance<WebMediator>();

        public class Config : WebApplicationBootstrapConfiguration { }

        public WebMediatorTests()
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
            config.RegistrationService = new Aspects.DI.Autofac.RegistrationService();
            config.CacheConfiguration = new InMemoryCacheConfiguration();
            config.Bootstrap(typeof(MediatorTests).Assembly);
        }

        [Fact]
        public void Do()
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/Process1";
            context.Request.Method = "GET";

            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            var request = new HttpContextWrapper(mockHttpContextAccessor.Object);

            var result = WebMediator.Dispatch(request);
        }
    }
}
