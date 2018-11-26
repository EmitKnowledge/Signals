using Signals.Aspects.Storage.Configurations;

namespace Signals.Aspects.Storage.Azure.Configurations
{
    /// <summary>
    /// Azure storage configuration
    /// </summary>
    public class AzureStorageConfiguration : IStorageConfiguration
    {
        /// <summary>
        /// Azure connection string
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
    }
}
