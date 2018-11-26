using System;
using Signals.Aspects.Auditing.Configurations;

namespace Signals.Aspects.Auditing.AuditNET.Configurations
{
    public class FileAuditingConfiguration : IAuditingConfiguration
    {
		/// <summary>
		/// CTOR
		/// </summary>
        public FileAuditingConfiguration()
        {
            DirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            FilenameBuilder = () => $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString()}.json";
        }

        /// <summary>
        /// The directory where the audit entry is stored
        /// </summary>
        public string DirectoryPath { get; set; }

        /// <summary>
        /// Function that returns the file where the audit entry is stored
        /// </summary>
        public Func<string> FilenameBuilder { get; set; }
    }
}