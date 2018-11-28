using System.Collections.Generic;
using System.Linq;

namespace App.Domain.Configuration.Database
{
    /// <summary>
    /// Configuration for database
    /// </summary>
    public class DatabaseConfigurationElement
    {
        /// <summary>
        /// Database configurations
        /// </summary>
        public List<DatabaseConfigurationElementItem> DatabaseConnections { get; set; }

        /// <summary>
        /// Active database configuration name
        /// </summary>
        public string ActiveEnvironment { get; set; }

        /// <summary>
        /// Provider for active database configuration
        /// </summary>
        public DatabaseConfigurationElementItem ActiveConfiguration => DatabaseConnections?.SingleOrDefault(x => x.Name == ActiveEnvironment);
    }
}