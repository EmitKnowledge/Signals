using Ganss.XSS;
using Microsoft.AspNetCore.Http;
using Moq;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Configuration;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Configuration;
using Signals.Core.Web.Configuration.Bootstrapping;
using Signals.Core.Web.Execution;
using Signals.Core.Web.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Signals.Tests.Core
{
    public class Input : IDtoData
    {
        public string Input1 { get; set; }
        public int Input2 { get; set; }
        public List<string> Input3 { get; set; }
        public Input5 Input4 { get; set; }

        public class Input5
        {
            public string Input6 { get; set; }
        }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
        }
    }

    [ApiProcess(HttpMethod = ApiProcessMethod.GET)]
    public class GetWebProcess : ApiProcess<Input, VoidResult>
    {
        public override VoidResult Auth(Input request)
        {
            return Ok();
        }

        public override VoidResult Validate(Input request)
        {
            return Ok();
        }

        public override VoidResult Handle(Input request)
        {
            if (request.Input1.IsNull()
                || request.Input2 == 0
                || request.Input3.IsNullOrHasZeroElements()
                || request.Input3.FirstOrDefault().IsNullOrEmpty()
                || request.Input4?.Input6.IsNullOrEmpty() != false)
                return Fail();

            return Ok();
        }
    }

    [ApiProcess(HttpMethod = ApiProcessMethod.POST)]
    public class PostWebProcess : ApiProcess<Input, VoidResult>
    {
        public override VoidResult Auth(Input request)
        {
            return Ok();
        }

        public override VoidResult Validate(Input request)
        {
            return Ok();
        }

        public override VoidResult Handle(Input request)
        {
            if (request.Input1.IsNull()
                || request.Input2 == 0
                || request.Input3.IsNullOrHasZeroElements()
                || request.Input3.FirstOrDefault().IsNullOrEmpty()
                || request.Input4?.Input6.IsNullOrEmpty() != false)
                return Fail();

            return Ok();
        }
    }

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
        public void WebMediator_GetRequest_NotFaulted()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/api/GetWebProcess";
            context.Request.Method = "GET";
            context.Request.QueryString = QueryString.FromUriComponent("?Input1=Input1&Input2=2&Input3[]=Input3&Input4[Input6]=Input6");

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            var request = new HttpContextWrapper(mockHttpContextAccessor.Object);

            // Act
            var result = WebMediator.Dispatch(request);

            // Asset
            var body = context.Response.Body;
            body.Position = 0;
            var bodyStr = new StreamReader(body).ReadToEnd();

            Assert.Contains("\"IsFaulted\":false", bodyStr);
        }

        [Fact]
        public void WebMediator_PostRequest_NotFaulted()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/api/PostWebProcess";
            context.Request.Method = "POST";
            context.Request.Body = new MemoryStream();

            var input = new Input
            {
                Input1 = "Input1",
                Input2 = 2,
                Input3 = new List<string> { "Input3" },
                Input4 = new Input.Input5 { Input6 = "Input6" }
            };

            var requestWriter = new StreamWriter(context.Request.Body);
            requestWriter.WriteLine(input.SerializeJson());
            requestWriter.Flush();
            context.Request.Body.Position = 0;

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            var request = new HttpContextWrapper(mockHttpContextAccessor.Object);

            // Act
            var result = WebMediator.Dispatch(request);

            // Asset
            var body = context.Response.Body;
            body.Position = 0;
            var bodyStr = new StreamReader(body).ReadToEnd();

            Assert.Contains("\"IsFaulted\":false", bodyStr);
        }
    }
}
