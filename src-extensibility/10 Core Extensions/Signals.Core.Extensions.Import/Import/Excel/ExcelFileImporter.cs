using Signals.Core.Processes.Import;
using Signals.Core.Extensions.Import.Configuration;
using Signals.Core.Processing.Results;
using System.IO;
using Signals.Core.Extensions.Import.Extensions;

namespace Signals.Core.Extensions.Import.Import.Excel
{
    /// <summary>
    /// File importer implementation for excel files
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public class ExcelFileImporter<TResponse> : IFileImporter<TResponse>
        where TResponse : new()
    {
        /// <summary>
        /// Returns imported data from the stream
        /// </summary>
        /// <param name="importConfiguration"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public ListResult<TResponse> Import(IImportConfiguration<TResponse> importConfiguration, Stream stream)
        {
            return new ListResult<TResponse>(ExcelExtensions.Import(stream, importConfiguration as ExcelImportConfiguration<TResponse>));
        }
    }
}