using Signals.Core.Processing.Results;
using Signals.Tests.Core.Web.Web;
using Xunit;

namespace Signals.Tests.Core.Web.Tests
{
    public class PingTest : TestFixture
    {
        [Fact]
        public void Autofac_SignalsPingTest()
        {
            Startup.ContainerName = "autofac";
            Init();

            var response = MockRequest<MethodResult<bool>>("api/web/api/ping");

            Assert.True(response.Result);
        }

        [Fact]
        public void Autofac_MvcPingTest()
        {
            Startup.ContainerName = "autofac";
            Init();

            var response = MockRequest<MethodResult<bool>>("ping/index");
            Assert.True(response.Result);
        }

        [Fact]
        public void SimpleInjector_SignalsPingTest()
        {
            Startup.ContainerName = "simpleinjector";
            Init();

            var response = MockRequest<MethodResult<bool>>("api/web/api/ping");

            Assert.True(response.Result);
        }

        [Fact]
        public void SimpleInjector_MvcPingTest()
        {
            Startup.ContainerName = "simpleinjector";
            Init();

            var response = MockRequest<MethodResult<bool>>("ping/index");
            Assert.True(response.Result);
        }

        [Fact]
        public void DotNetCore_SignalsPingTest()
        {
            Startup.ContainerName = "dotnetcore";
            Init();

            var response = MockRequest<MethodResult<bool>>("api/web/api/ping");

            Assert.True(response.Result);
        }

        [Fact]
        public void DotNetCore_MvcPingTest()
        {
            Startup.ContainerName = "dotnetcore";
            Init();
            var response = MockRequest<MethodResult<bool>>("ping/index");
            Assert.True(response.Result);
        }
    }
}
