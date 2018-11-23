using Signals.Aspects.Storage.Configurations;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Signals.Aspects.Storage.Helpers
{
    /// <summary>
    /// Certificate provider helper
    /// </summary>
    /// <returns></returns>
    public static class CertHelper
    {
        /// <summary>
        /// Provide certificate
        /// </summary>
        /// <param name="thumbprint"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificate(string thumbprint, StoreLocation location)
        {
            thumbprint = Regex.Replace(thumbprint, @"[^\da-zA-z]", string.Empty).ToUpper();
            var store = new X509Store(StoreName.My, location);

            try
            {
                store.Open(OpenFlags.ReadOnly);

                var certCollection = store.Certificates;
                var signingCert = certCollection.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (signingCert.Count == 0)
                {
                    return null;
                }

                return signingCert[0];
            }
            finally
            {
                store.Close();
            }
        }
        
        /// <summary>
        /// Provide certificate
        /// </summary>
        /// <param name="thumbprint"></param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificate(string thumbprint)
        {
            var cert = GetCertificate(thumbprint, StoreLocation.CurrentUser) ?? GetCertificate(thumbprint, StoreLocation.LocalMachine);
	        return cert;
        }
        
        /// <summary>
        /// Provide certificate
        /// </summary>
        /// <param name="certPath"></param>
        /// <param name="certPass"></param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificate(string certPath, string certPass)
        {
	        var cert = new X509Certificate2(File.ReadAllBytes(certPath), certPass, X509KeyStorageFlags.MachineKeySet);
            cert.Verify();
	        return cert;
        }

		/// <summary>
		/// Provide certificate from base configuration
		/// </summary>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public static X509Certificate2 GetCertificate(this IStorageConfiguration configuration)
        {

            if (!configuration.Encrypt) return null;
            if (!string.IsNullOrEmpty(configuration.CertificateThumbprint))
            {
                return GetCertificate(configuration.CertificateThumbprint);
            }
            if (!string.IsNullOrEmpty(configuration.CertificatePassword))
            {
                return GetCertificate(configuration.CertificatePath, configuration.CertificatePassword);
            }
            return null;
        }
    }
}