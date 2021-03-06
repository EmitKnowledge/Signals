﻿using Signals.Core.Processes.Import;
using System;
using System.Collections.Generic;

namespace Signals.Core.Extensions.Import.Configuration
{
    /// <summary>
    /// Represents importing configuration model
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public class ImportConfiguration<TResponse> : IImportConfiguration<TResponse>
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public ImportConfiguration()
        {
            DataHandlers = new Dictionary<string[], Action<string[], TResponse>>();
        }

        /// <summary>
        /// Represents dictionary of column name(s) and data handler action that stores the extracted data object
        /// </summary>
        public Dictionary<string[], Action<string[], TResponse>> DataHandlers { get; set; }
    }
}
