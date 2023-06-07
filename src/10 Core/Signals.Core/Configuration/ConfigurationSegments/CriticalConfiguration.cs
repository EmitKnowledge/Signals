using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Signals.Core.Configuration.ConfigurationSegments
{
    /// <summary>
    /// Critical process configuration
    /// </summary>
    public class CriticalConfiguration
    {
        /// <summary>
        /// Person to contact in case of emergency
        /// </summary>
        [Required]
        public List<string> Emails { get; set; }

        /// <summary>
        /// Email subject
        /// </summary>
        [Required]
        public string Subject { get; set; }

        /// <summary>
        /// Email body
        /// </summary>
        [Required]
        public string Body { get; set; }
    }
}
