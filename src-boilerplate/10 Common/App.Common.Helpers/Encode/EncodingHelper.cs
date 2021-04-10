using System.Text;

namespace App.Common.Helpers.Encode
{
    public static class EncodingHelper
    {
        /// <summary>
        /// Return danish encoding
        /// </summary>
        /// <returns></returns>
        public static Encoding GetDanishEncoding() => Encoding.GetEncoding("ISO-8859-1");
    }
}