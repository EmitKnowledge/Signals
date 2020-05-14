using Signals.Features.Base.Configurations.MicroService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Features.Base.Configurations.Feature
{
    /// <summary>
    /// Base feature registration
    /// </summary>
    public interface IFeatureConfiguration
    {
        MicroServiceConfiguration MicroServiceConfiguration { get; set; }
    }
}
