using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Signals.Core.Common.Serialization.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Signals.Core.Common.Serialization.Json
{
    /// <summary>
    /// JSON serializer
    /// </summary>
    public class JsonSerializer : ISerializer
    {
        /// <summary>
        /// Deserialize string to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Deserialize string to object
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        /// <summary>
        /// Serialize string to object
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public string Serialize(object instance)
        {
            if (instance == null) return null;
            var serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter());
            serializer.Converters.Add(new StreamConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.StringEscapeHandling = StringEscapeHandling.Default;
            

            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    using (var writer = new JsonTextWriter(sw))
                    {
                        writer.Formatting = Formatting.None;
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
