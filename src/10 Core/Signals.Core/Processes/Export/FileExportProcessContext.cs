using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processes.Export
{
    /// <summary>
    /// File export process context
    /// </summary>
    public class FileExportProcessContext : BaseProcessContext
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="process"></param>
        public FileExportProcessContext(IBaseProcess<VoidResult> process) : base(process)
        {
        }
    }
}