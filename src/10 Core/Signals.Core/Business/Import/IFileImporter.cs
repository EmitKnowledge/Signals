using System.IO;
using Signals.Core.Processing.Results;

namespace Signals.Core.Business.Import
{
    /// <summary>
    /// File importing handler interface
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IFileImporter<TResponse>
    {
        /// <summary>
        /// Returns imported data from the stream
        /// </summary>
        /// <param name="importConfiguration"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        ListResult<TResponse> Import(IImportConfiguration<TResponse> importConfiguration, Stream stream);
    }
}