using Audit.Core;
using System;

namespace Signals.Aspects.Auditing.AuditNET
{
    internal class SignalsAuditEvent : AuditEvent
    {
        /// <summary>
        /// Represents the invoking process id
        /// </summary>
        public string Process { get; set; }

        /// <summary>
        /// Represents the invoking process instance id
        /// </summary>
        public string ProcessInstanceId { get; set; }

        /// <summary>
        /// Represents additional data about the audit entry
        /// </summary>
        public AuditEntryData Data { get; set; }

        /// <summary>
        /// Represents the originator
        /// </summary>
        public string Originator { get; set; }

        /// <summary>
        /// Represents the payload data
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Represents the process correlation id
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Implicitly convert AuditTrail to SignalsAuditEvent
        /// </summary>
        /// <param name="auditEntry"></param>
        public static implicit operator SignalsAuditEvent(AuditEntry auditEntry)
        {
            if (auditEntry == null) return null;

            return new SignalsAuditEvent
            {
                Process = auditEntry.Process,
                ProcessInstanceId = auditEntry.ProcessInstanceId,
                Originator = auditEntry.Originator,
                EventType = auditEntry.EventType,
                StartDate = auditEntry.StartDate,
                EndDate = auditEntry.EndDate,
                Data = auditEntry.Data,
                Payload = auditEntry.Payload,
                CorrelationId = auditEntry.CorrelationId
			};
        }

    }
}
