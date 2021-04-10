using System.ComponentModel.DataAnnotations;

namespace Signals.Tests.Configuration.CustomConfigurations.MsSql.Controllers.ExternalApis
{
    public class ExternalApi
    {
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
