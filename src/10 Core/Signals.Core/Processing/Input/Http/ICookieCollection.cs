using Signals.Core.Common.Instance;
using System;
using Signals.Core.Common.Serialization;

namespace Signals.Core.Processing.Input.Http
{
    /// <summary>
    /// Wrapper around real cookie collection
    /// </summary>
    public interface ICookieCollection
    {
        /// <summary>
        /// Add cookie to response
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        void Add(string name, object value, DateTime expiration);

        /// <summary>
        /// Remove cookie from response
        /// </summary>
        /// <param name="name"></param>
        void Remove(string name);

        /// <summary>
        /// Return cookie by name from request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T Get<T>(string name) where T : class;
    }
}
