﻿using Signals.Core.Processes.Export;
using System;
using System.Collections.Generic;

namespace Signals.Core.Extensions.ImportExport.Configuration.Export
{
    /// <summary>
    /// Represents exporing configuration model
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ExportConfiguration<TData> : IExportConfiguration<TData>
    {
        /// <summary>
        /// Represents the exporing file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Represents dictionary of column name and data mapper func for exporing data of type <see cref="TData"/>
        /// </summary>
        public Dictionary<string, Func<TData, object>> DataMapper { get; set; }
    }
}
