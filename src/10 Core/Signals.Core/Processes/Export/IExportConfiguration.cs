using System;
using System.Collections.Generic;

namespace Signals.Core.Processes.Export
{
    /// <summary>
    /// Data export configuration
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IExportConfiguration<TData>
    {
        /// <summary>
        /// Represents the exporing file name
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Represents dictionary of column name and data mapper func for exporing data
        /// </summary>
        Dictionary<string, Func<TData, object>> DataMapper { get; set; }
    }
}