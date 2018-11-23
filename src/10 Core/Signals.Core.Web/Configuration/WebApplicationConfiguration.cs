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
        /// CTOR
        /// </summary>
        public WebApplicationConfiguration()
        {
            ResponseHeaders = new List<ResponseHeaderAttribute>();
        }

        /// <summary>
        /// Config entry point
        /// </summary>
        /// <param name="entryAssembly"></param>
        /// <returns></returns>
        internal IServiceContainer Bootstrap(Assembly entryAssembly)
        {
            return Resolve(entryAssembly);
        }

        /// <summary>
        /// Build instances from configurations by convention
        /// </summary>
        /// <returns></returns>
        protected override IServiceContainer Resolve(Assembly entryAssembly)
        {
            RegistrationService.Register<IHttpContextWrapper, HttpContextWrapper>();
            RegistrationService.Register<IHttpContextAccessor, HttpContextAccessor>();
            RegistrationService.Register<WebMediator>();
            RegistrationService.Register<List<ResponseHeaderAttribute>>(ResponseHeaders);

            return base.Resolve(entryAssembly);
        }
    }
}