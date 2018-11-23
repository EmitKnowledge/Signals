﻿using Signals.Core.Business.Export;
using Signals.Core.Extensions.ImportExport.Configuration.Export;
using Signals.Core.Extensions.ImportExport.Extensions;
using System.Collections.Generic;
using System.IO;

namespace Signals.Core.Extensions.ImportExport.Export.Pdf
{
    /// <summary>
    /// File exporting handler
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class PdfFileExporter<TData> : IFileExporter<TData>
    {
        public string MimeType => "application/pdf";

        /// <summary>
        /// Exports the provided data in format defined in the export configuration as pdf file
        /// </summary>
        /// <param name="exportConfiguration"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Stream Export(IExportConfiguration<TData> exportConfiguration, List<TData> data)
        {
            return PdfExtensions.Export(exportConfiguration as PdfExportConfiguration<TData>, data);
        }
    }
}