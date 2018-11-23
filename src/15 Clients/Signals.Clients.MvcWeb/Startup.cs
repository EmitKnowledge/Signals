using Autofac.Integration.Mvc;
using Microsoft.Owin;
using Owin;
using Signals.Aspects.Auditing.AuditNET.Configurations;
using Signals.Aspects.Auth;
using Signals.Aspects.Auth.Mvc5.CustomScheme;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.CommunicationChannels.ServiceBus.Configurations;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI.SimpleInjector;
using Signals.Aspects.ErrorHandling.Polly;
using Signals.Aspects.Localization.File.Configurations;
using Signals.Aspects.Logging.NLog.Configurations;
using Signals.Aspects.Security.Database.Configurations;
using Signals.Aspects.Storage.Database.Configurations;
using Signals.Core.Configuration;
using Signals.Core.Web.Behaviour;
using Signals.Core.Web.Configuration;
using Signals.Core.Web.Extensions;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(Signals.Clients.Mvc5.Startup))]

namespace Signals.Clients.Mvc5
{
    public class CCustomPrincipalProvider : SignalsPrincipalProvider
	{
		private readonly IAuthorizationManager manager;

		public CCustomPrincipalProvider(IAuthorizationManager manager)
		{
			this.manager = manager;
		}

		public static ClaimsPrincipal Principal =>
			new ClaimsPrincipal(
				new ClaimsIdentity(
					new List<Claim> {
						new Claim(ClaimTypes.Name, "Custom Vojdan")
					},
				"Signals")
			);

		public override ClaimsPrincipal GetPrincipal(HttpRequest httpRequest)
		{
			var p = Principal;
			this.manager.AddRoles(p, "Admin", "SuperAdmin");
			return null;
		}
	}

	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
            var registrationService = new Signals.Aspects.DI.SimpleInjector.RegistrationService();
            //var registrationService = new Signals.Aspects.DI.Autofac.RegistrationService();

            ApplicationInformation
                .UseProvider(new FileConfigurationProvider
                {
                    Path = @"E:\repos\Emit.Knowledge.Signals\trunk\src\15 Clients\Signals.Clients.MvcWeb",
                    File = "appsettings.json",
                    ReloadOnAccess = false
                });

            WebInformation
                .UseProvider(new FileConfigurationProvider
                {
                    Path = @"E:\repos\Emit.Knowledge.Signals\trunk\src\15 Clients\Signals.Clients.MvcWeb",
                    File = "websettings.json",
                    ReloadOnAccess = false
                });

            app.MapSignals(config =>
            {
                config.ResponseHeaders = new List<ResponseHeaderAttribute> { new ContentSecurityPolicyAttribute() };
                config.RegistrationService = registrationService;
                config.StrategyBuilder = new StrategyBuilder();
                config.AuditingConfiguration = new FileAuditingConfiguration();
                config.LoggerConfiguration = new DatabaseLoggingConfiguration();
                config.CacheConfiguration = new InMemoryCacheConfiguration();
                config.ChannelConfiguration = new ServiceBusChannelConfiguration
                {
                    ConnectionString = $@"Endpoint=sb://esb-envoice-test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=HzPc16dfugFbBAdW+0vfKtHDJ+sjg8I/GD/cJEPXupk="
                };
                config.LocalizationConfiguration = new JsonDataProviderConfiguration();
                config.StorageConfiguration = new DatabaseStorageConfiguration();
                config.SecurityConfiguration = new DatabaseSecurityConfiguration();
            });

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(registrationService.Builder));
            //DependencyResolver.SetResolver(new AutofacDependencyResolver((registrationService.Build() as Signals.Aspects.DI.Autofac.ServiceContainer).Container));
        }
	}
}
