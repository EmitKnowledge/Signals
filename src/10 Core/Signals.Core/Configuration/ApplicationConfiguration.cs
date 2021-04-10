using Signals.Aspects.Configuration;
using Signals.Core.Configuration.ConfigurationSegments;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Signals.Core.Configuration
{
    /// <summary>
    /// Base application information
    /// </summary>
    public class ApplicationConfiguration : BaseConfiguration<ApplicationConfiguration>
    {
        /// <summary>
        /// Configuration key
        /// </summary>
        public override string Key => nameof(ApplicationConfiguration);

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
        /// Person to contact in case of emergency
        /// </summary>
        public List<string> WhitelistedEmails { get; set; }

        /// <summary>
        /// Person to contact in case of emergency
        /// </summary>
        public List<string> WhitelistedEmailDomains { get; set; }

        /// <summary>
        /// Critical process data
        /// </summary>
        public CriticalConfiguration CriticalConfiguration { get; set; }

        /// <summary>
        /// Smtp configuration
        /// </summary>
        public SmtpConfiguration SmtpConfiguration { get; set; }
    }
}
