using System;

namespace Signals.Aspects.Auditing
{
    public interface IAuditProvider
    {
        /// <summary>
        /// Creates audit for the audit entry
        /// </summary>
        /// <param name="auditEntry"></param>
        /// <param name="action"></param>
        void Audit(AuditEntry auditEntry, Action action);

        /// <summary>
        /// Creates audit for the audit entry
        /// </summary>
        /// <param name="auditEntry"></param>
        /// <param name="action"></param>
        T Audit<T>(AuditEntry auditEntry, Func<T> action);

        /// <summary>
        /// Creates new audit entry and returns it
        /// </summary>
        /// <returns></returns>
        AuditEntry Entry();

    }
}