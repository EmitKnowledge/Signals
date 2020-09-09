using Signals.Features.Base.Configurations.MicroService;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Signals.Features.Hosting")]
namespace Signals.Features.Base.Configurations.Feature
{
    /// <summary>
    /// Base feature configuration
    /// </summary>
    public class BaseFeatureConfiguration
    {
        /// <summary>
        /// Feature as micro service configuration
        /// </summary>
        internal MicroServiceConfiguration MicroServiceConfiguration { get; set; }
    }
}
