using System.ComponentModel;

namespace Signals.Aspects.Logging.Enums
{
    /// <summary>
    /// Data providers
    /// </summary>
    public enum DataProvider
    {
        /// <summary>
        /// Sql client data provider
        /// </summary>
        [Description("System.Data.SqlClient")]
        SqlClient = 1
    }
}
