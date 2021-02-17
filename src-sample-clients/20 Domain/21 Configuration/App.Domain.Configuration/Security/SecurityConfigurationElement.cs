using System.ComponentModel.DataAnnotations;

namespace App.Domain.Configuration.Security
{
    public sealed class SecurityConfigurationElement
    {
        /// <summary>
        /// Lenght of the generated salt
        /// </summary>
        [Required]
        public int SaltLenght { get; set; }

        /// <summary>
        /// Password lenght of the automatic generated password
        /// </summary>
        [Required]
        public int AutoPasswordLenght { get; set; }

        /// <summary>
        /// Minimum lenght of the password
        /// </summary>
        [Required]
        public int MinPasswordLenght { get; set; }

        /// <summary>
        /// Lenght of the generated token
        /// </summary>
        [Required]
        public int TokenLenght { get; set; }

        /// <summary>
        /// Lenght of the generated token
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