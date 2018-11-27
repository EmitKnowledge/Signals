using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace App.Service.Configuration.Storage
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
