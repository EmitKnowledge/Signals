using Signals.Aspects.Configuration;
using Signals.Aspects.Configuration.MsSql;
using Signals.Aspects.Configuration.Validations;
using Signals.Tests.Configuration.CustomConfigurations.MsSql.Controllers.ExternalApis;

namespace Signals.Tests.Configuration.CustomConfigurations.MsSql.Controllers
{
    public class ControllersConfiguration : BaseConfiguration<ControllersConfiguration>
    {
        public override string Key => nameof(ControllersConfiguration);

        [ValidateObject]
        public ApplicationConfiguration ApplicationConfiguration { get; set; }

        [ValidateObject]
        public SecurityConfiguration SecurityConfiguration { get; set; }

        [ValidateObject]
        public ExternalApisConfiguration ExternalApisConfiguration { get; set; }

    }
}
