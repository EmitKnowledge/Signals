using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processes.Business
{
    /// <summary>
    /// Business process context
    /// </summary>
    public class BusinessProcessContext : BaseProcessContext
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="process"></param>
        public BusinessProcessContext(IBaseProcess<VoidResult> process) : base(process)
        {
        }
    }
}