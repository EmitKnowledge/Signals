using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Signals.Tests.Core.Web.Web;

namespace Signals.Tests.Core.Web
{
    public static class Program
    {
        public static void Main(params string[] args)
        {
            if (string.IsNullOrEmpty(Startup.ContainerName))
                Startup.ContainerName = "simpleinjector";
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls(args)
                .UseStartup<Startup>();
    }
}
