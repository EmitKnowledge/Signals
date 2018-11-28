using Signals.Aspects.Security.Configurations;

namespace Signals.Aspects.Security.Database.Configurations
{
    /// <summary>
    /// Condifuration for database
    /// </summary>
    public class DatabaseSecurityConfiguration : ISecurityConfiguration
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public DatabaseSecurityConfiguration()
        {
            TableName = "Permission";
        }

        /// <summary>
        /// Represents the connection string to the database with permission table
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Represents the permissions table name in the database
        /// </summary>
        public string TableName { get; set; }
    }
}
