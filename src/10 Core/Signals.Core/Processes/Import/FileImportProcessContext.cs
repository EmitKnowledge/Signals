using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processes.Import
{
    /// <summary>
    /// File import process context
    /// </summary>
    public interface IFileImportProcessContext : IBaseProcessContext
    {
    }

    /// <summary>
    /// File import process context
    /// </summary>
    [Export(typeof(IFileImportProcessContext))]
    public class FileImportProcessContext : BaseProcessContext, IFileImportProcessContext
    {
    }
}