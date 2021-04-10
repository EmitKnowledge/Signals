using System.Collections;
using System.Collections.Generic;

namespace Signals.Core.Processing.Input.Http
{
    /// <summary>
    /// Wrapper around real form collection
    /// </summary>
    public interface IFormCollection : IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        /// <summary>
        /// Return the value for the provided key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object this[string key] { get; }

        /// <summary>
        /// Return form keys count
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Return all of the available keys
        /// </summary>
        ICollection<string> Keys { get; }

        /// <summary>
        /// Check if the provided key exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(string key);
       
        /// <summary>
        /// Try to get a value for the provided key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetValue(string key, out object value);
    }
}
