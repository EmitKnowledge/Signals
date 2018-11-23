namespace Signals.Aspects.Storage.Configurations
{
    /// <summary>
    /// Storage configuration
    /// </summary>
    public interface IStorageConfiguration
    {
        /// <summary>
        /// Should stored files be encrypted
        /// </summary>
        bool Encrypt { get; set; }

        /// <summary>
        /// Certificate path for ecntrypting files
        /// </summary>
        string CertificatePath { get; set; }
        
        /// <summary>
        /// Certificate password for ecntrypting files
        /// </summary>
        string CertificatePassword { get; set; }
        
        /// <summary>
        /// Certificate thumbprint for ecntrypting files
        /// </summary>
        string CertificateThumbprint { get; set; }
    }
}
