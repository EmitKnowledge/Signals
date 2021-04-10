using System.ComponentModel.DataAnnotations;

namespace App.Domain.Configuration.Database
{
    /// <summary>
    /// Defines database configuration element
    /// </summary>
    public sealed class DatabaseConfigurationElementItem
    {
        /// <summary>
        /// Environment name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Server address
        /// </summary>
        [Required]
        public string IpAddress { get; set; }

        /// <summary>
        /// Connection string database
        /// </summary>
        [Required]
        public string Database { get; set; }

        /// <summary>
        /// Connection string user id
        /// </summary>
        [Required]
        public string Uid { get; set; }

        /// <summary>
        /// Connection string password
        /// </summary>
        [Required]
        public string Pwd { get; set; }

        /// <summary>
        /// Create valid mongodb connection string
        /// </summary>
        public string ConnectionString => ToString();

        public override string ToString() => string.Format("server={2};Database={3};User Id={0};Password = {1}", Uid, Pwd, IpAddress, Database);
    }
}