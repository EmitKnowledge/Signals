using Signals.Core.Processing.Results;
using Signals.Tests.Core.Web.Web;
using Xunit;

namespace Signals.Tests.Core.Web.Tests
{
    public class AddNumbersTest : TestFixture
    {
        [Fact]
        public void Autofac_AddNumbersTest()
        {
            Startup.ContainerName = "autofac";
            Init();

            var response = MockRequest<MethodResult<int>>("api/web/api/addNumbers", new { A = 1, B = 2 });

            Assert.Equal(3, response.Result);
        }

        [Fact]
        public void SimpleInjector_AddNumbersTest()
        {
            Startup.ContainerName = "simpleinjector";
            Init();

            var response = MockRequest<MethodResult<int>>("api/web/api/addNumbers", new { A = 1, B = 2 });

            Assert.Equal(3, response.Result);
        }

        [Fact]
        public void DotNetCore_AddNumbersTest()
        {
            Startup.ContainerName = "dotnetcore";
            Init();

            var response = MockRequest<MethodResult<int>>("api/web/api/addNumbers", new { A = 1, B = 2 });

            Assert.Equal(3, response.Result);
        }
    }
}
