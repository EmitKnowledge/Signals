using System;
using System.Collections.Generic;

namespace Signals.Core.Business.Export
{
    public interface IExportConfiguration<TData>
    {
        /// <summary>
        /// Represents the exporing file name
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Represents dictionary of column name and data mapper func for exporing data of type <see cref="TData"/>
        /// </summary>
        Dictionary<string, Func<TData, object>> DataMapper { get; set; }
    }
}