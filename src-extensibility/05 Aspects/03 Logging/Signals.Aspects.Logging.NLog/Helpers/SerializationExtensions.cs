using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Text;

namespace Signals.Aspects.Logging.NLog.Helpers
{
	internal static class SerializationExtensions
    {
        /// <summary>
        /// Serialize an instance to json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="shouldIdent"></param>
        /// <param name="includeTypes"></param>
        /// <param name="escapeHtml"></param>
        /// <returns></returns>
        public static string Serialize<T>(this T instance, bool shouldIdent = true, bool includeTypes = false, bool escapeHtml = false)
        {
            if (instance == null) return null;
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.StringEscapeHandling = escapeHtml ? StringEscapeHandling.EscapeHtml : StringEscapeHandling.Default;
            serializer.TypeNameHandling = includeTypes ? TypeNameHandling.All : TypeNameHandling.None;
            serializer.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;

            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    using (var writer = new JsonTextWriter(sw))
                    {
                        writer.Formatting = shouldIdent ? Formatting.Indented : Formatting.None;
                        serializer.Serialize(writer, instance);
                        sw.Flush();
                        var json = Encoding.UTF8.GetString(ms.ToArray());
                        return json;
                    }
                }
            }
        }
    }
}
