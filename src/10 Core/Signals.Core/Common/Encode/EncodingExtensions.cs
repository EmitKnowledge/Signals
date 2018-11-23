using System;
using System.IO;

namespace Signals.Core.Common.Encode
{
    public static class EncodingExtensions
    {
        /// <summary>
        /// Retrieve byte array from a string (UTF8 encoding used as default)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string value)
        {
            return ToBytes(value, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Retrieve string from a given byte array (UTF8 encoding used as default)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FromBytes(this byte[] value)
        {
            return FromBytes(value, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Retrieve byte array from a string and a given encoding
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string value, System.Text.Encoding encoding)
        {
            return encoding.GetBytes(value);
        }

        /// <summary>
        /// Retrieve string from a given byte array
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string FromBytes(this byte[] value, System.Text.Encoding encoding)
        {
            return encoding.GetString(value);
        }

        /// <summary>
        /// Generate stream from string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MemoryStream FromString(this string value)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(value);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Generate stream from string
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string FromStream(this MemoryStream stream)
        {
            stream.Position = 0;
            var reader = new StreamReader(stream, System.Text.Encoding.UTF8, true);
            var data = reader.ReadToEnd();
            return data;
        }

        /// <summary>
        /// Convert to url friendly base64 string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes).Replace("=", "").Replace('+', '-').Replace('/', '_');
        }

        /// <summary>
        /// Decode url friendly base64 to byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] FromBase64(this string value)
        {
            value = value.Replace('-', '+').Replace('_', '/');
            string padding = new string('=', 3 - (value.Length + 3) % 4);
            value += padding;
            return Convert.FromBase64String(value);
        }
    }
}