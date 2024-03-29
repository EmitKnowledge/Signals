﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization.Json.Converters;
using System;
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
            return (T)DeserializeObject(json, typeof(T));
        }

        /// <summary>
        /// Deserialize string to object
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Deserialize(string json, Type type)
        {
            return DeserializeObject(json, type);
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

            var settings = JsonConvert.DefaultSettings;
            if (!settings.IsNull() && !settings().IsNull()) serializer = Newtonsoft.Json.JsonSerializer.Create(settings());

            serializer.Converters.Add(new IsoDateTimeConverter());
            serializer.Converters.Add(new StreamConverter());
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

        /// <summary>
        /// Deserialize object to string
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private object DeserializeObject(string json, Type type)
        {
            json = json?.Trim('\n', '\t', ' ');
            if (json.IsNullOrEmpty()) return json;
            
            var settings = JsonConvert.DefaultSettings?.Invoke() ?? new Newtonsoft.Json.JsonSerializerSettings();

            return JsonConvert.DeserializeObject(json, type, settings);                
        }
    }
}
