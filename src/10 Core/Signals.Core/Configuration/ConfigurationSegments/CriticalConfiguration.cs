using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Signals.Core.Configuration.ConfigurationSegments
{
    /// <summary>
    /// Critical process configuraiton
    /// </summary>
    public class CriticalConfiguration
    {
        /// <summary>
        /// Person to contact in case of emergency
        /// </summary>
        [EmailAddress]
        [Required]
        public string OwnerEmail { get; set; }

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
