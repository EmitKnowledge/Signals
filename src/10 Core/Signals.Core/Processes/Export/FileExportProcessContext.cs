using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processes.Export
{
    /// <summary>
    /// File export process context
    /// </summary>
    public interface IFileExportProcessContext : IBaseProcessContext
    {
    }

    /// <summary>
    /// File export process context
    /// </summary>
    [Export(typeof(IFileExportProcessContext))]
    public class FileExportProcessContext : BaseProcessContext, IFileExportProcessContext
    {
    }
}