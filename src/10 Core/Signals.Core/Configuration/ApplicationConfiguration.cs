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
        /// Application email
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

        private bool _enableVerbose = false;

        /// <summary>
        /// Indicates if Signals VERBOSE should be enabled
        /// This will result in usage of: System.Diagnostics.Trace.WriteLine through the framework
        /// Verbose output can be configured via the web and app config files.
        /// </summary>
        public bool EnableVerbose
        {
	        get => _enableVerbose;
	        set
	        {
		        _enableVerbose = value;
		        Debugging.TracingEnabled = _enableVerbose;
	        }
        }
    }
}
