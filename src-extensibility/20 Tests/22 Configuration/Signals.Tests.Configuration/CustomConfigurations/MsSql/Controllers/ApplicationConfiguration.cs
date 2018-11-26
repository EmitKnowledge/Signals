using System.ComponentModel.DataAnnotations;

namespace Signals.Tests.Configuration.CustomConfigurations.MsSql.Controllers
{
    public class ApplicationConfiguration
    {
        public string ProjectName { get; set; }

        [Required]
        public string ApplicationName { get; set; }

        public string Environment { get; set; }
    }
}
