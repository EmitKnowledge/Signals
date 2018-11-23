using System.Collections.Generic;

namespace Signals.Aspects.Auditing
{
    public class AuditEntryData
    {
        public AuditEntryData()
        {
            Source = new Dictionary<string, object>();
            Process = new Dictionary<string, object>();
            ProcessInstance = new Dictionary<string, object>();
        }

        /// <summary>
        /// Holds information about the audit entry source
        /// </summary>
        public Dictionary<string, object> Source { get; set; }

        /// <summary>
        /// Holds information about the audit entry process
        /// </summary>
        public Dictionary<string, object> Process { get; set; }

        /// <summary>
        /// Holds information about the audit entry process instance
        /// </summary>
        public Dictionary<string, object> ProcessInstance { get; set; }
    }
}
