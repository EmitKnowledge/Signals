using System.ComponentModel.DataAnnotations;

namespace App.Domain.Configuration.Localization
{
    /// <summary>
    /// Windows azure service bus configuration
    /// </summary>
    public sealed class LocalizationConfigurationElement
    {
        /// <summary>
        /// Cookie key for localization
        /// </summary>
        [Required]
        public string CookieKey { get; set; }

        /// <summary>
        /// Default localization culture
        /// </summary>
        [Required]
        public string DefaultCulture { get; set; }
    }
}