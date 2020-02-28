using Signals.Aspects.DI;
using Signals.Core.Configuration.Bootstrapping;
using Signals.Core.Web.Behaviour;
using System.Collections.Generic;
using System.Reflection;

namespace Signals.Core.Web.Configuration.Bootstrapping
{
    /// <summary>
    /// Aspects configuration
    /// </summary>
    public class FluentWebApplicationBootstrapConfiguration : FluentApplicationBootstrapConfiguration, IWebApplicationBootstrapConfiguration
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
        public FluentWebApplicationBootstrapConfiguration()
        {
            ResponseHeaders = new List<ResponseHeaderAttribute>();
            ScanAssemblies = new List<Assembly>();
        }

        /// <summary>
        /// Default response headers
        /// </summary>
        public FluentWebApplicationBootstrapConfiguration AddDefaultResponseHeader(ResponseHeaderAttribute responseHeader)
        {
            ResponseHeaders.Add(responseHeader);
            return this;
        }

        /// <summary>
        /// Application assemblies to be scanned for processes and type exports
        /// </summary>
        public FluentWebApplicationBootstrapConfiguration RegisterAssempliesForScaning(List<Assembly> scanAssemblies)
        {
            ScanAssemblies = scanAssemblies;
            return this;
        }

        /// <summary>
        /// Build instances from configurations by convention
        /// </summary>
        /// <param name="scanAssemblies"></param>
        /// <returns></returns>
        public override IServiceContainer Bootstrap(params Assembly[] scanAssemblies)
        {
            return this.BootstrapHelper(scanAssemblies);
        }
    }
}