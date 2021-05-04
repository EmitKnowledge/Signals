using Signals.Aspects.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Tests.Core.Web.Web.config
{
    /// <summary>
    /// Configuration for database
    /// </summary>
    public class DatabaseConfiguration : BaseConfiguration<DatabaseConfiguration>
    {
        public override string Key => GetType().Name;

        /// <summary>
        /// Database configurations
        /// </summary>
        public List<DatabaseConfigurationItem> DatabaseConnections { get; set; }

        /// <summary>
        /// Active database configuration name
        /// </summary>
        public string ActiveEnvironment { get; set; }

        /// <summary>
        /// Provider for active database configuration
        /// </summary>
        public DatabaseConfigurationItem ActiveConfiguration => DatabaseConnections?.SingleOrDefault(x => x.Name == ActiveEnvironment);
    }
}