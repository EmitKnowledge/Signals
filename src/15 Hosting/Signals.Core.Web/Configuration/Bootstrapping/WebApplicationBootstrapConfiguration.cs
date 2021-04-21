using Microsoft.AspNetCore.Http;
using Signals.Aspects.DI;
using Signals.Core.Common.Instance;
using Signals.Core.Configuration;
using Signals.Core.Configuration.Bootstrapping;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Web.Behaviour;
using Signals.Core.Web.Execution;
using Signals.Core.Web.Http;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Signals.Core.Web.Configuration.Bootstrapping
{
    internal interface IWebApplicationBootstrapConfiguration : IApplicationBootstrapConfiguration
    {
        /// <summary>
        /// Default response headers
        /// </summary>
        List<ResponseHeaderAttribute> ResponseHeaders { get; set; }

        /// <summary>
        /// Application assemblies to be scanned for processes and type exports
        /// </summary>
        List<Assembly> ScanAssemblies { get; set; }
    }

    internal static class webApplicationBootstrapConfigurationExtensions
    {
        /// <summary>
        /// Bootstrapping entry
        /// </summary>
        /// <param name="webBootstrapConfiguration"></param>
        /// <param name="scanAssemblies"></param>
        /// <returns></returns>
        public static IServiceContainer BootstrapHelper(this IWebApplicationBootstrapConfiguration webBootstrapConfiguration, params Assembly[] scanAssemblies)
        {
            // Proc config validation
            WebApplicationConfiguration config = null;
            try
            {
                config = WebApplicationConfiguration.Instance;
            }
            catch { }
            finally
            {
                if (config.IsNull()) throw new Exception("Signals.Core.Web.Configuration.WebApplicationConfiguration is not provided. Please use a configuration provider to provide configuration values!");
            }

            webBootstrapConfiguration.RegistrationService.Register<IHttpContextAccessor, HttpContextAccessor>();
            webBootstrapConfiguration.RegistrationService.Register<IHttpContextWrapper, HttpContextWrapper>();
            webBootstrapConfiguration.RegistrationService.RegisterSingleton<WebMediator>();
            webBootstrapConfiguration.RegistrationService.Register(webBootstrapConfiguration.ResponseHeaders);

            return webBootstrapConfiguration.Resolve(scanAssemblies: scanAssemblies);
        }
    }

    /// <summary>
    /// Aspects configuration
    /// </summary>
    public class WebApplicationBootstrapConfiguration : FluentApplicationBootstrapConfiguration, IWebApplicationBootstrapConfiguration
    {
        /// <summary>
        /// Default response headers
        /// </summary>
        public List<ResponseHeaderAttribute> ResponseHeaders { get; set; }

        /// <summary>
        /// Application assemblies to be scanned for processes and type exports
        /// </summary>
        public List<Assembly> ScanAssemblies { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public WebApplicationBootstrapConfiguration()
        {
            ResponseHeaders = new List<ResponseHeaderAttribute>();
            ScanAssemblies = new List<Assembly>();
        }

        /// <summary>
        /// Config entry point
        /// </summary>
        /// <param name="scanAssemblies"></param>
        /// <returns></returns>
        public override IServiceContainer Bootstrap(params Assembly[] scanAssemblies)
        {
            return this.BootstrapHelper(scanAssemblies);
        }
    }
}