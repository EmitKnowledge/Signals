using System.Collections.Generic;
using Signals.Aspects.Configuration.Validations;

namespace Signals.Tests.Configuration.CustomConfigurations.MsSql.Controllers.ExternalApis
{
    public class ExternalApisConfiguration
    {
        [ValidateObject]
        public List<ExternalApi> ExternalApis { get; set; }
    }
}
