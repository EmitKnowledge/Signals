using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization.Json;
using Signals.Core.Common.Serialization.Xml;
using System;
using System.ComponentModel;

namespace Signals.Core.Common.Serialization
{
    /// <summary>
    /// Available serialization options
    /// </summary>
    public enum SerializationFormat
    {
        /// <summary>
        /// Detect based on content
        /// </summary>
        [Description("text/text")]
        Detect,

        /// <summary>
        /// JSON
        /// </summary>
        [Description("application/json")]
        Json,

        /// <summary>
        /// XML
        /// </summary>
        [Description("application/xml")]
        Xml
    }

    /// <summary>
    /// Object serialization extensions
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Json serializer
        /// </summary>
        public static ISerializer JsonSerializer = new JsonSerializer();

        /// <summary>
        /// Xml serializer
        /// </summary>
        public static ISerializer XmlSerializer = new XmlSerializer();

        /// <summary>
        /// Resolve serializer by content or option
        /// </summary>
        /// <param name="input"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private static ISerializer ResolveSerializer(string input, SerializationFormat format)
        {
            if (format == SerializationFormat.Json)
                return JsonSerializer;
            if (format == SerializationFormat.Xml)
                return XmlSerializer;

            if(!input.IsNullOrEmpty())
            {
                if (input.StartsWith("{") && input.EndsWith("}") || input.StartsWith("[") && input.EndsWith("]"))
                    return JsonSerializer;
                if (input.StartsWith("<") && input.EndsWith(">"))
                    return XmlSerializer;
            }

            return null;
        }

        /// <summary>
        /// Serialize json
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string SerializeJson(this object instance)
        {
            return Serialize(instance, SerializationFormat.Json);
        }

        /// <summary>
        /// Serialize xml
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string SerializeXml(this object instance)
        {
            return Serialize(instance, SerializationFormat.Xml);
        }

        /// <summary>
        /// Serialize with option
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private static string Serialize(this object instance, SerializationFormat format = SerializationFormat.Detect)
        {
            if (format == SerializationFormat.Detect) throw new Exception("Provide valid serialization format");

            var serializer = ResolveSerializer(null, format);
            return serializer?.Serialize(instance);
        }

        /// <summary>
        /// Deserialize with options
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string str, SerializationFormat format = SerializationFormat.Detect)
        {
            var serializer = ResolveSerializer(str, format);
            if (serializer == null) return default(T);
            return serializer.Deserialize<T>(str);
        }

        /// <summary>
        /// Deserialize with options
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static object Deserialize(this string str, Type type, SerializationFormat format = SerializationFormat.Detect)
        {
            var serializer = ResolveSerializer(str, format);
            return serializer?.Deserialize(str, type);
        }
    }
}