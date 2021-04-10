using System.Collections.Generic;
using System.IO;

namespace Signals.Core.Processes.Export
{
    /// <summary>
    /// File exporting handler interface
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IFileExporter<TData>
    {
        /// <summary>
        /// Mime type
        /// </summary>
        string MimeType { get; }

        /// <summary>
        /// Exports the provided data in format defined in the export configuration
        /// </summary>
        /// <param name="exportConfiguration"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Stream Export(IExportConfiguration<TData> exportConfiguration, List<TData> data);
    }
}