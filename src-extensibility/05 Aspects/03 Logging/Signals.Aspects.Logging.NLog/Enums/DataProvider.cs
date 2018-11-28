using System.ComponentModel;

namespace Signals.Aspects.Logging.NLog.Enums
{
    /// <summary>
    /// Database providers
    /// </summary>
    public enum DataProvider
    {
        /// <summary>
        /// MSSql provider
        /// </summary>
        [Description("System.Data.SqlClient")]
        SqlClient = 1
    }
}
