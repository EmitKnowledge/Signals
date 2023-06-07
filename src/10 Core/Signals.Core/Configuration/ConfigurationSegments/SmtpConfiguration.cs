using System.ComponentModel.DataAnnotations;

namespace Signals.Core.Configuration.ConfigurationSegments
{
    /// <summary>
    /// SMTP account configuration
    /// </summary>
    public class SmtpConfiguration
    {
        /// <summary>
        /// SMTP server
        /// </summary>
        [Required]
        public string Server { get; set; }

        /// <summary>
        /// SMTP server port
        /// </summary>
        [Required]
        public int Port { get; set; }

        /// <summary>
        /// Account user name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Account password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Should use ssl
        /// </summary>
        public bool UseSsl { get; set; }
    }
}
