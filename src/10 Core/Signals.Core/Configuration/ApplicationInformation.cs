using Signals.Aspects.Configuration;
using Signals.Core.Configuration.ConfigurationSegments;
using System.ComponentModel.DataAnnotations;

namespace Signals.Core.Configuration
{
    /// <summary>
    /// Base application information
    /// </summary>
    public class ApplicationInformation : BaseConfiguration<ApplicationInformation>
    {
        /// <summary>
        /// Configuration key
        /// </summary>
        public override string Key => nameof(ApplicationInformation);

        /// <summary>
        /// Application name, e.g. Facebook website
        /// </summary>
        [Required]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Application version, e.g. 1.0.0
        /// </summary>
        [Required]
        public string ApplicationVersion { get; set; }

        /// <summary>
        /// Applicaiton email
        /// </summary>
        [EmailAddress]
        [Required]
        public string ApplicationEmail { get; set; }

        /// <summary>
        /// Critical process data
        /// </summary>
        public CriticalConfiguration CriticalConfiguration { get; set; }

        /// <summary>
        /// Critical process data
        /// </summary>
        public SmtpConfiguration SmtpConfiguration { get; set; }
    }
}
