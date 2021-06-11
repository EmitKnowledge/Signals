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
using Signals.Core.Web.Behaviour;
using Signals.Core.Web.Configuration;
using Signals.Core.Web.Configuration.Bootstrapping;
using Signals.Core.Web.Execution;
using Signals.Core.Web.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

    [SignalsApi(HttpMethod = SignalsApiMethod.GET)]
    [ResponseHeader("custom-header", "custom-value")]
    public class GetWebProcess : ApiProcess<Input, MethodResult<int>>
    {
        public override MethodResult<int> Auth(Input request)
        {
            return Ok();
        }

        public override MethodResult<int> Validate(Input request)
        {
            return Ok();
        }

        public override MethodResult<int> Handle(Input request)
        {
            if (request.Input1.IsNull()
                || request.Input2 == 0
                || request.Input3.IsNullOrHasZeroElements()
                || request.Input3.FirstOrDefault().IsNullOrEmpty()
                || request.Input4?.Input6.IsNullOrEmpty() != false)
                return Fail();

            return 1;
        }
    }

    [SignalsApi(HttpMethod = SignalsApiMethod.GET)]
    [Cors("custom-origin")]
    public class GetCorsWebProcess : ApiProcess<Input, MethodResult<int>>
    {
        public override MethodResult<int> Auth(Input request)
        {
            return Ok();
        }

        public override MethodResult<int> Validate(Input request)
        {
            return Ok();
        }

        public override MethodResult<int> Handle(Input request)
        {
            if (request.Input1.IsNull()
                || request.Input2 == 0
                || request.Input3.IsNullOrHasZeroElements()
                || request.Input3.FirstOrDefault().IsNullOrEmpty()
                || request.Input4?.Input6.IsNullOrEmpty() != false)
                return Fail();

            return 1;
        }
    }

    [SignalsApi(HttpMethod = SignalsApiMethod.POST)]
    public class PostWebProcess : ApiProcess<Input, MethodResult<int>>
    {
        public override MethodResult<int> Auth(Input request)
        {
            return Ok();
        }

        public override MethodResult<int> Validate(Input request)
        {
            return Ok();
        }

        public override MethodResult<int> Handle(Input request)
        {
            if (request.Input1.IsNull()
                || request.Input2 == 0
                || request.Input3.IsNullOrHasZeroElements()
                || request.Input3.FirstOrDefault().IsNullOrEmpty()
                || request.Input4?.Input6.IsNullOrEmpty() != false)
                return Fail();

            return 1;
        }
    }

    public class WebMediatorTests
    {
        /// <summary>
        /// Web mediator
        /// </summary>
        private WebMediator WebMediator => SystemBootstrapper.GetInstance<WebMediator>();

        public class Config : WebApplicationBootstrapConfiguration { }

        /// <summary>
        /// CTOR
        /// </summary>
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
            config.RegistrationService = new Aspects.DI.SimpleInjector.RegistrationService();
            config.CacheConfiguration = new InMemoryCacheConfiguration();
            config.Bootstrap(typeof(MediatorTests).Assembly);
        }

        /// <summary>
        /// Mock and send web request to api process
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="method"></param>
        /// <param name="queryString"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private T MockRequest<T>(string path, SignalsApiMethod method, out DefaultHttpContext context, string queryString = null, object body = null)
            where T : VoidResult
        {
            // build http context
            context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = path;
            context.Request.Method = method.ToString();

            if (!queryString.IsNullOrEmpty())
                context.Request.QueryString = QueryString.FromUriComponent(queryString);

            if (!body.IsNull())
                context.Request.Body = new MemoryStream(Encoding.Default.GetBytes(body.SerializeJson()));

            // mock context accessor
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            var request = new HttpContextWrapper(mockHttpContextAccessor.Object);
            
            // dispatch mock web request
            WebMediator.Dispatch(request);

            // read response
            context.Response.Body.Position = 0;
            var response = new StreamReader(context.Response.Body).ReadToEnd();
            return response.Deserialize<T>();
        }

        [Fact]
        public void WebMediator_CorsOptionsRequest_NotFaulted()
        {
            // Arrange
            var path = "/api/GetCorsWebProcess";
            var method = SignalsApiMethod.OPTIONS;
            var queryString = "?Input1=Input1&Input2=2&Input3[]=Input3&Input4[Input6]=Input6";

            // Act
            var response = MockRequest<MethodResult<int>>(path, method, out DefaultHttpContext context, queryString: queryString);

            // Asset
            Assert.Equal(3, context.Response.Headers.Count);
            Assert.Equal("Access-Control-Allow-Origin", context.Response.Headers.FirstOrDefault().Key);
            Assert.Equal("custom-origin", context.Response.Headers.FirstOrDefault().Value);
        }

        [Fact]
        public void WebMediator_OptionsRequest_NotFaulted()
        {
            // Arrange
            var path = "/api/GetWebProcess";
            var method = SignalsApiMethod.OPTIONS;
            var queryString = "?Input1=Input1&Input2=2&Input3[]=Input3&Input4[Input6]=Input6";

            // Act
            var response = MockRequest<MethodResult<int>>(path, method, out DefaultHttpContext context, queryString: queryString);

            // Asset
            Assert.Equal(1, context.Response.Headers.Count);
            Assert.Equal("Allow", context.Response.Headers.FirstOrDefault().Key);
            Assert.Equal("GET", context.Response.Headers.FirstOrDefault().Value);
        }

        [Fact]
        public void WebMediator_HeadRequest_NotFaulted()
        {
            // Arrange
            var path = "/api/GetWebProcess";
            var method = SignalsApiMethod.HEAD;
            var queryString = "?Input1=Input1&Input2=2&Input3[]=Input3&Input4[Input6]=Input6";

            // Act
            var response = MockRequest<MethodResult<int>>(path, method, out DefaultHttpContext context, queryString: queryString);

            // Asset
            Assert.Equal(2, context.Response.Headers.Count);
            Assert.Equal("custom-header", context.Response.Headers.FirstOrDefault().Key);
            Assert.Equal("custom-value", context.Response.Headers.FirstOrDefault().Value);
            Assert.Equal("Content-Type", context.Response.Headers.LastOrDefault().Key);
            Assert.Equal("application/json", context.Response.Headers.LastOrDefault().Value);
        }

        [Fact]
        public void WebMediator_GetRequest_NotFaulted()
        {
            // Arrange
            var path = "/api/GetWebProcess";
            var method = SignalsApiMethod.GET;
            var queryString = "?Input1=Input1&Input2=2&Input3[]=Input3&Input4[Input6]=Input6";

            // Act
            var response = MockRequest<MethodResult<int>>(path, method, out DefaultHttpContext context, queryString: queryString);

            // Asset
            Assert.False(response.IsFaulted);
            Assert.Equal(1, response.Result);
        }

        [Fact]
        public void WebMediator_PostRequest_NotFaulted()
        {
            // Arrange
            var path = "/api/PostWebProcess";
            var method = SignalsApiMethod.POST;

            var input = new Input
            {
                Input1 = "Input1",
                Input2 = 2,
                Input3 = new List<string> { "Input3" },
                Input4 = new Input.Input5 { Input6 = "Input6" }
            };

            // Act
            var response = MockRequest<MethodResult<int>>(path, method, out DefaultHttpContext context, body: input);
            
            // Asset
            Assert.False(response.IsFaulted);
            Assert.Equal(1, response.Result);
        }
    }
}
