using System.ComponentModel.DataAnnotations;

namespace App.Domain.Configuration.Web
{
    public sealed class WebConfigurationElement
    {
        /// <summary>
        /// Web endpoing url
        /// </summary>
        [Required]
        public string Url { get; set; }

        /// <summary>
        /// Maximum page size that can be returned by the API
        /// </summary>
        [Required]
        public int MaxPageSize { get; set; }
    }
}