using Signals.Aspects.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Signals.Core.Web.Configuration
{
    /// <summary>
    /// Web application information
    /// </summary>
    public class WebApplicationConfiguration : BaseConfiguration<WebApplicationConfiguration>
    {
        /// <summary>
        /// Configuration key
        /// </summary>
        public override string Key => nameof(WebApplicationConfiguration);

        /// <summary>
        /// Web applicaiton url
        /// </summary>
        [Required]
        [RegularExpression(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*(\.[a-z]{2,5})?(:[0-9]{1,5})?(\/.*)?$")]
        public string WebUrl { get; set; }
    }
}
