using System;

namespace Signals.Aspects.Auditing
{
    public class AuditEntry
    {
        public AuditEntry(string processInstanceId)
        {
            ProcessInstanceId = processInstanceId;
			StartDate = DateTime.UtcNow;
            Data = new AuditEntryData();
        }

        /// <summary>
        /// Represents the invoking process
        /// </summary>
        public string Process { get; set; }

        /// <summary>
        /// Represents the invoking process instance id
        /// </summary>
        public string ProcessInstanceId { get; }

        /// <summary>
        /// Represents the event type
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Represents the start datetime
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Represents the end datetime
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Represents the originator
        /// </summary>
        public string Originator { get; set; }

        /// <summary>
        /// Represents additional data about the audit entry
        /// </summary>
        public AuditEntryData Data { get; set; }

    }
}
