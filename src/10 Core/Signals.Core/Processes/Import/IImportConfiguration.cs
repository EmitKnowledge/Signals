using System;
using System.Collections.Generic;

namespace Signals.Core.Processes.Import
{
    /// <summary>
    /// Represents importing configuration model
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IImportConfiguration<TResponse>
    {
        /// <summary>
        /// Represents dictionary of column name(s) and data handler action that stores the extracted data to the response object
        /// </summary>
        Dictionary<string[], Action<string[], TResponse>> DataHandlers { get; set; }
    }
}