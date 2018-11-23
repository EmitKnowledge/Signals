using Microsoft.AspNetCore.Http;
using Signals.Aspects.DI;
using Signals.Core.Configuration;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Web.Behaviour;
using Signals.Core.Web.Execution;
using Signals.Core.Web.Http;
using System.Collections.Generic;
using System.Reflection;

namespace Signals.Core.Web.Configuration
{
    /// <summary>
    /// Aspects configuration
    /// </summary>
    public class WebApplicationConfiguration : ApplicationConfiguration
    {
        /// <summary>
        /// Default response headers
        /// </summary>
        public List<ResponseHeaderAttribute> ResponseHeaders { get; set; }

        /// <summary>
        /// Applization assemblies to be scanned for processes and type exports
        /// </summary>
        public List<Assembly> ScanAssemblies { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public WebApplicationConfiguration()
        {
            ResponseHeaders = new List<ResponseHeaderAttribute>();
            ScanAssemblies = new List<Assembly>();
        }

        /// <summary>
        /// Config entry point
        /// </summary>
        /// <param name="entryAssembly"></param>
        /// <returns></returns>
        internal IServiceContainer Bootstrap(params Assembly[] scanAssemblies)
        {
            return Resolve(scanAssemblies);
        }

        /// <summary>
        /// Build instances from configurations by convention
        /// </summary>
        /// <returns></returns>
        protected override IServiceContainer Resolve(params Assembly[] scanAssemblies)
        {
            RegistrationService.Register<IHttpContextWrapper, HttpContextWrapper>();
            RegistrationService.Register<IHttpContextAccessor, HttpContextAccessor>();
            RegistrationService.Register<WebMediator>();
            RegistrationService.Register<List<ResponseHeaderAttribute>>(ResponseHeaders);

            return base.Resolve(scanAssemblies);
        }
    }
}