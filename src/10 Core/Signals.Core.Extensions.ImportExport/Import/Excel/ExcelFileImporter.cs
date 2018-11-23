using Signals.Core.Business.Import;
using Signals.Core.Extensions.ImportExport.Configuration.Import;
using Signals.Core.Extensions.ImportExport.Extensions;
using Signals.Core.Processing.Results;
using System.IO;

namespace Signals.Core.Extensions.ImportExport.Import.Excel
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