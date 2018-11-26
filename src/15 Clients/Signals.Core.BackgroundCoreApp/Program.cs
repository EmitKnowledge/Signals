using Signals.Aspects.Auditing.AuditNET.Configurations;
using Signals.Aspects.BackgroundProcessing.FluentScheduler;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.CommunicationChannels.ServiceBus.Configurations;
using Signals.Aspects.DI.DotNetCore;
using Signals.Aspects.ErrorHandling.Polly;
using Signals.Aspects.Logging.NLog.Configurations;
using Signals.Aspects.Security.Database.Configurations;
using Signals.Aspects.Storage.Database.Configurations;
using Signals.Core.Background.Configuration;
using Signals.Core.Background.Configuration.Bootstrapping;
using Signals.Core.Configuration;
using System;
using System.Reflection;

namespace Signals.Core.BackgroundCoreApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ApplicationConfiguration.UseProvider(new Signals.Aspects.Configuration.File.FileConfigurationProvider { File = @"appsettings.json" });

            var dependencyInjector = new RegistrationService();

            var config = new BackgroundApplicationBootstrapConfiguration
            {
                RegistrationService = dependencyInjector,
                StrategyBuilder = new StrategyBuilder(),
                TaskRegistry = new FluentRegistry(),
                AuditingConfiguration = new FileAuditingConfiguration(),
                LoggerConfiguration = new DatabaseLoggingConfiguration(),
                CacheConfiguration = new InMemoryCacheConfiguration(),
                ChannelConfiguration = new ServiceBusChannelConfiguration
                {
                    ConnectionString = $@"Endpoint=sb://esb-envoice-test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=HzPc16dfugFbBAdW+0vfKtHDJ+sjg8I/GD/cJEPXupk="
                },
                StorageConfiguration = new DatabaseStorageConfiguration(),
                SecurityConfiguration = new DatabaseSecurityConfiguration()
            };

            config.Bootstrap(Assembly.GetExecutingAssembly());

            Console.Read();
        }
    }
}
