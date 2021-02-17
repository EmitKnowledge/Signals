using System.ComponentModel.DataAnnotations;

namespace App.Domain.Configuration.Security
{
    public sealed class SecurityConfigurationElement
    {
        /// <summary>
        /// length of the generated salt
        /// </summary>
        [Required]
        public int SaltLength { get; set; }

        /// <summary>
        /// Password length of the automatic generated password
        /// </summary>
        [Required]
        public int AutoPasswordLength { get; set; }

        /// <summary>
        /// Minimum length of the password
        /// </summary>
        [Required]
        public int MinPasswordLength { get; set; }

        /// <summary>
        /// length of the generated token
        /// </summary>
        [Required]
        public int TokenLength { get; set; }

        /// <summary>
        /// length of the generated token
        /// </summary>
        [Required]
        public int TokenValidityInDays { get; set; }

        /// <summary>
        /// Encryption secret
        /// </summary>
        [Required]
        public string Fish { get; set; }
    }
}