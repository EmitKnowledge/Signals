﻿using Signals.Aspects.Storage.Configurations;

namespace Signals.Aspects.Storage.Database.Configurations
{
    /// <summary>
    /// Database storage configuration
    /// </summary>
    public class DatabaseStorageConfiguration : IStorageConfiguration
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public DatabaseStorageConfiguration()
        {
            TableName = "Storage";
        }

        /// <summary>
        /// Database connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Should stored files be encrypted
        /// </summary>
        public bool Encrypt { get; set; }

        /// <summary>
        /// Certificate path for ecntrypting files
        /// </summary>
        public string CertificatePath { get; set; }

        /// <summary>
        /// Certificate password for ecntrypting files
        /// </summary>
        public string CertificatePassword { get; set; }

        /// <summary>
        /// Certificate thumbprint for ecntrypting files
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        /// File storage table name
        /// </summary>
        public string TableName { get; set; }
    }
}
