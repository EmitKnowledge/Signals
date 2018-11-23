using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Signals.Core.Common.Serialization.Xml
{
    /// <summary>
    /// XML serializer
    /// </summary>
    public class XmlSerializer : ISerializer
    {
        /// <summary>
        /// Deserialize string to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public T Deserialize<T>(string str)
        {
            return (T)Deserialize(str, typeof(T));
        }

        /// <summary>
        /// Deserialize string to object
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Deserialize(string str, Type type)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(type);
            var xmlReader = XmlReader.Create(new StringReader(str));
            return serializer.Deserialize(xmlReader);
        }

        /// <summary>
        /// Serialize string to object
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public string Serialize(object instance)
        {
            StringBuilder output = new StringBuilder();
            var serializer = new System.Xml.Serialization.XmlSerializer(instance.GetType());
            var xmlWriter = XmlWriter.Create(new StringWriter(output));

            return output.ToString();
        }
    }
}
