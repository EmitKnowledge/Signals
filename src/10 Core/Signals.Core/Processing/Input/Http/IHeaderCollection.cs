using System;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Core.Processing.Input.Http
{
    /// <summary>
    /// Wrapper around real header collection
    /// </summary>
    public interface IHeaderCollection
    {
        /// <summary>
        /// Add header to response
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void AddToResponse(string name, string value);

        /// <summary>
        /// Removes all headers from response
        /// </summary>
        void RemoveAllFromResponse();

        /// <summary>
        /// Get headers from response
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetFromResponse(string name);

        /// <summary>
        /// Gets headers from request
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetFromRequest(string name);
    }
}
