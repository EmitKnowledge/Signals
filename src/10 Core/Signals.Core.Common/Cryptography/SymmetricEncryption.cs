using Signals.Core.Common.Encode;
using System.IO;
using System.Security.Cryptography;

namespace Signals.Core.Common.Cryptography
{
    /// <summary>
    /// Helper for symmetric encryption using AES
    /// </summary>
    public static class SymmetricEncryption
    {
        /// <summary>
        /// Encrypt data with the provided key
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            byte[] encryptedBytes;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = { 7, 8, 2, 0, 1, 6, 7, 9 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.Padding = PaddingMode.ISO10126;
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var rfc2898Key = new Rfc2898DeriveBytes(key, saltBytes, 1000);
                    aes.Key = rfc2898Key.GetBytes(aes.KeySize / 8);
                    aes.IV = rfc2898Key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        /// <summary>
        /// Descrypt data with the provided key
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] data, byte[] key)
        {
            byte[] decryptedBytes;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = { 7, 8, 2, 0, 1, 6, 7, 9 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.Padding = PaddingMode.ISO10126;
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var rfc2898Key = new Rfc2898DeriveBytes(key, saltBytes, 1000);
                    aes.Key = rfc2898Key.GetBytes(aes.KeySize / 8);
                    aes.IV = rfc2898Key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        /// <summary>
        /// Encrypt data with the provided key
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string data, string key)
        {
            // Get the bytes of the string
            byte[] bytesToBeEncrypted = System.Text.Encoding.UTF8.GetBytes(data);
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(key);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = Encrypt(bytesToBeEncrypted, passwordBytes);
            string result = bytesEncrypted.ToBase64();

            return result;
        }

        /// <summary>
        /// Descrypt data with the provided key
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string data, string key)
        {
            // Get the bytes of the string
            byte[] bytesToBeDecrypted = data.FromBase64();
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(key);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = Decrypt(bytesToBeDecrypted, passwordBytes);

            var result = System.Text.Encoding.UTF8.GetString(bytesDecrypted);
            return result;
        }
    }
}