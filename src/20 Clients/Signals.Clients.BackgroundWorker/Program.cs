using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections;
using System.Diagnostics;

namespace App.Clients.BackgroundWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || ((IList)args).Contains("--console"));

            var builder = new HostBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddHostedService<Service>();
                });

            if (isService)
            {
                builder.RunAsServiceAsync().GetAwaiter().GetResult();
            }
            else
            {
                builder.RunConsoleAsync().GetAwaiter().GetResult();
            }
        }
    }
}