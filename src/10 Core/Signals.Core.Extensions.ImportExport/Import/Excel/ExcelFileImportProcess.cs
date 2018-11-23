using Signals.Core.Business.Import;
using System.IO;

namespace Signals.Core.Extensions.ImportExport.Import.Excel
{
    /// <summary>
    /// Represents process for importing data of type <see cref="TResponse"/> from excel stream
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    public abstract class ExcelFileImportProcess<TInput, TResponse> : BaseFileImportProcess<TInput, TResponse>
        where TInput : Stream
        where TResponse : new()
    {
        /// <summary>
        /// Defines file importer that will handle the importing
        /// </summary>
        /// <returns></returns>
        protected override IFileImporter<TResponse> ResolveFileImporter()
        {
            return new ExcelFileImporter<TResponse>();
        }
    }
}