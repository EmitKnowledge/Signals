using System.ComponentModel.DataAnnotations;

namespace App.Domain.Configuration.Storage
{
    /// <summary>
    /// Defines storage configuration element
    /// </summary>
    public sealed class StorageConfigurationElement
    {
        /// <summary>
        /// Storage name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Connection string to storage
        /// </summary>
        [Required]
        public string ConnectionString { get; set; }
    }
}