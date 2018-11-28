using Signals.Aspects.Auditing.Configurations;

namespace Signals.Aspects.Auditing.AuditNET.Configurations
{
    /// <summary>
    /// Database configuration for auditing
    /// </summary>
    public class DatabaseAuditingConfiguration : IAuditingConfiguration
    {
        /// <summary>
        /// The connection string to the database where the audit entries are stored
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The database table where the audit logs are stored
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Default CTOR
        /// </summary>
        public DatabaseAuditingConfiguration()
        {
            TableName = "AuditEntry";
        }
    }
}