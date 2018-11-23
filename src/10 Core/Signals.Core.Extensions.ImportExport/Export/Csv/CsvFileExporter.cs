﻿using Signals.Core.Business.Export;
using Signals.Core.Extensions.ImportExport.Configuration.Export;
using Signals.Core.Extensions.ImportExport.Extensions;
using System.Collections.Generic;
using System.IO;

namespace Signals.Core.Extensions.ImportExport.Export.Csv
{
    /// <summary>
    /// File exporting handler
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class CsvFileExporter<TData> : IFileExporter<TData>
    {
        public string MimeType => "text/csv";

        /// <summary>
        /// Exports the provided data in format defined in the export configuration as csv file
        /// </summary>
        /// <param name="exportConfiguration"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Stream Export(IExportConfiguration<TData> exportConfiguration, List<TData> data)
        {
            return CsvExtensions.Export(exportConfiguration as ExportConfiguration<TData>, data);
        }
    }
}