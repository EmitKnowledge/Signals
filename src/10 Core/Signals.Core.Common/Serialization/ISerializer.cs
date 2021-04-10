using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Core.Common.Serialization
{
    /// <summary>
    /// Data serializer
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serialize object
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        string Serialize(object instance);

        /// <summary>
        /// Deserialize object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        T Deserialize<T>(string str);

        /// <summary>
        /// Deserialize object
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object Deserialize(string str, Type type);
    }
}
