using Signals.Aspects.Storage.Configurations;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Signals.Aspects.Storage.Helpers
{
    /// <summary>
    /// Encryption helper
    /// </summary>
    public static class EncryptionHelper
    {
		/// <summary>
		/// Encrypt stream using certificate
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		[Obsolete]
        public static Stream Encrypt(this IStorageConfiguration configuration, Stream input)
        {
            Stream output = new MemoryStream();
            int privateLength = 8;
            string publicKey = null;
            string privateKey = null;

            if (!configuration.Encrypt)
            {
                output.Position = 0;
                input.Position = 0;

                input.CopyTo(output);

                output.Position = 0;
                input.Position = 0;

                return output;
            }

            if (!string.IsNullOrEmpty(configuration.CertificateThumbprint))
            {
                publicKey = string.Join("", configuration.CertificateThumbprint.OrderBy(x => x).Take(privateLength));
                privateKey = string.Join("", configuration.CertificateThumbprint.Take(privateLength));
            }
            else if (!string.IsNullOrEmpty(configuration.CertificatePassword))
            {
                publicKey = string.Join("", configuration.CertificatePassword.OrderBy(x => x).Take(privateLength));
                privateKey = string.Join("", configuration.CertificatePassword.Take(privateLength));
            }

            byte[] data = new byte[input.Length];
            input.Position = 0;
            input.Read(data, 0, data.Length);
            input.Position = 0;
            data = Pad(data, privateLength);

	        if (string.IsNullOrEmpty(publicKey)) throw new ArgumentException("Public key is not valid", nameof(publicKey));
	        if (string.IsNullOrEmpty(privateKey)) throw new ArgumentException("Private key is not valid", nameof(privateKey));

			// Setup the encryption object
			DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
            cryptic.Key = Encoding.ASCII.GetBytes(publicKey);
            cryptic.IV = Encoding.ASCII.GetBytes(privateKey);
            cryptic.Padding = PaddingMode.None;

            using (Stream tempStream = new MemoryStream())
            {
                // Create a crptographic output stream for the already open file output stream
                using (CryptoStream crStream = new CryptoStream(tempStream, cryptic.CreateEncryptor(), CryptoStreamMode.Write))
                {

                    // Write the data buffer to the encryption stream, then close everything
                    crStream.Write(data, 0, data.Length);

                    output.Position = 0;
                    tempStream.Position = 0;

                    tempStream.CopyTo(output);

                    output.Position = 0;
                }
            }

            return output;
        }

		/// <summary>
		/// Decrypt stream using certificate
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		[Obsolete]
        public static Stream Decrypt(this IStorageConfiguration configuration, Stream input)
        {
            Stream output = new MemoryStream();
            int privateLength = 8;
            string publicKey = null;
            string privateKey = null;

            if (!configuration.Encrypt)
            {
                output.Position = 0;
                input.Position = 0;

                input.CopyTo(output);

                output.Position = 0;
                input.Position = 0;

                return output;
            }
	        if (!string.IsNullOrEmpty(configuration.CertificateThumbprint))
	        {
		        publicKey = string.Join("", configuration.CertificateThumbprint.OrderBy(x => x).Take(privateLength));
		        privateKey = string.Join("", configuration.CertificateThumbprint.Take(privateLength));
	        }
	        else if (!string.IsNullOrEmpty(configuration.CertificatePassword))
	        {
		        publicKey = string.Join("", configuration.CertificatePassword.OrderBy(x => x).Take(privateLength));
		        privateKey = string.Join("", configuration.CertificatePassword.Take(privateLength));
	        }

			if (string.IsNullOrEmpty(publicKey)) throw new ArgumentException("Public key is not valid", nameof(publicKey));
	        if (string.IsNullOrEmpty(privateKey)) throw new ArgumentException("Private key is not valid", nameof(privateKey));

			DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
            cryptic.Key = Encoding.ASCII.GetBytes(publicKey);
            cryptic.IV = Encoding.ASCII.GetBytes(privateKey);
            cryptic.Padding = PaddingMode.None;

            byte[] data = new byte[input.Length];

            using (Stream tempStream = new MemoryStream())
            {
                input.Position = 0;
                tempStream.Position = 0;

                input.CopyTo(tempStream);

                input.Position = 0;
                tempStream.Position = 0;

                using (CryptoStream crStream = new CryptoStream(tempStream, cryptic.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (BinaryReader reader = new BinaryReader(crStream))
                    {
                        reader.Read(data, 0, data.Length);
                    }
                }
            }

            data = Trim(data);

            output.Write(data, 0, data.Length);

            input.Position = 0;
            output.Position = 0;

            return output;
        }

        private static byte[] Trim(byte[] input)
        {
            if (input == null || input.Length == 0) return input;

            var bytes = input.ToList();
            bytes.Reverse();
            var padding = bytes.First(x => x != 0);

            bytes.Reverse();
            var result = bytes.Take(bytes.Count - padding).ToArray();
            return result;
        }

        private static byte[] Pad(byte[] input, int chunk)
        {
            if (input == null || input.Length == 0) return input;

            var toAdd = chunk;
            var bytes = input.ToList();
            if (bytes.Count % chunk != 0)
            {
                toAdd = chunk - (bytes.Count % chunk);
            }

            bytes.Add((byte)toAdd);
            for (int i = 0; i < toAdd - 1; i++)
            {
                bytes.Add(0);
            }

            var result = bytes.ToArray();
            return result;
        }
    }
}
