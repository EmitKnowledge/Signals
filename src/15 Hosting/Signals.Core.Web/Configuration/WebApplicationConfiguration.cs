using Signals.Aspects.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.Core.Web.Configuration
{
    /// <summary>
    /// Base application information
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
