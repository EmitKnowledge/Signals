using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Features.Base.Configurations
{
    public interface IFeatureConfiguration
    {
        MicroServiceConfiguration MicroServiceConfiguration { get; set; }
    }
}
