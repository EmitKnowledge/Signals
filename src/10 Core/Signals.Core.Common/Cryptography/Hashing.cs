using Signals.Core.Common.Encode;
using Signals.Core.Common.Instance;
using System.Security.Cryptography;
using System.Text;

namespace Signals.Core.Common.Cryptography
{
    /// <summary>
    /// Helper for hashing
    /// </summary>
    public static class Hashing
    {
        /// <summary>
        /// Calculates MD5 hash from a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToMd5(byte[] input)
        {
            var md5Provider = MD5.Create();
            var bytes = md5Provider.ComputeHash(input);

            var hashBuilder = new StringBuilder();

            for (var byteIndex = 0; byteIndex < bytes.Length; byteIndex++)
            {
                hashBuilder.Append(bytes[byteIndex].ToString("x2"));
            }

            return hashBuilder.ToString();
        }

        /// <summary>
        /// Calculates MD5 hash from a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToMd5(string input)
        {
            return ToMd5(input.ToBytes());
        }

        /// <summary>
        /// Verify MD5 hash with a given input string
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool VerifyMd5(string hash, string input)
        {
            var inputHash = ToMd5(input);
            return hash.IsEqual(inputHash);
        }

        /// <summary>
        /// Computes 128 bit Sha1 from string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSha1(string input)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] bytes = sha.ComputeHash(input.ToBytes());

            var hashBuilder = new StringBuilder();

            for (var byteIndex = 0; byteIndex < bytes.Length; byteIndex++)
            {
                hashBuilder.Append(bytes[byteIndex].ToString("x2"));
            }

            return hashBuilder.ToString();
        }

        /// <summary>
        /// Computes base64 from string
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string ToBase64(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Computes Sha256 from string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ToSha256(string input, string key)
        {
            var keyByte = System.Text.Encoding.UTF8.GetBytes(key);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                hmacsha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                var bytes = hmacsha256.Hash;
                var hashBuilder = new StringBuilder();

                for (var byteIndex = 0; byteIndex < bytes.Length; byteIndex++)
                {
                    hashBuilder.Append(bytes[byteIndex].ToString("x2"));
                }
                return hashBuilder.ToString();
            }
        }

        /// <summary>
        /// Verifies Sha1 hash with input
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool VerifySha1(string hash, string input)
        {
            var inputHash = ToSha1(input);
            return hash.IsEqual(inputHash);
        }

        /// <summary>
        /// Verifies Sha1 hash with input
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool VerifySha256(string hash, string input, string key)
        {
            var inputHash = ToSha256(input, key);
            return hash.IsEqual(inputHash);
        }

        /// <summary>
        /// Generates salt for usage in hash
        /// generation to increase hash security
        /// </summary>
        /// <param name="saltLen"></param>
        /// <returns></returns>
        public static string GenerateSalt(int saltLen = 8)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[saltLen];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(saltLen);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        /// <summary>
        /// Computes string from base64
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public static string FromBase64(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}