using System;
using Signals.Aspects.Storage.Configurations;

namespace Signals.Aspects.Storage.File.Configurations
{
    /// <summary>
    /// File storage configuration
    /// </summary>
    public class FileStorageConfiguration : IStorageConfiguration
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public FileStorageConfiguration()
        {
            Encrypt = false;
            RootPath = Environment.CurrentDirectory;
        }

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
        /// Root directory path where all files are stored
        /// </summary>
        public string RootPath { get; set; }
    }
}
