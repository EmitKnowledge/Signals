using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processes.Import
{
    /// <summary>
    /// File import process context
    /// </summary>
    public class FileImportProcessContext : BaseProcessContext
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="process"></param>
        public FileImportProcessContext(IBaseProcess<VoidResult> process) : base(process)
        {
        }
    }
}