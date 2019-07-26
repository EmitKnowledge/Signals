using Signals.Aspects.DI;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processes.Api
{
    /// <summary>
    /// Api process context
    /// </summary>
    public class ApiProcessContext : BaseProcessContext
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="process"></param>
        public ApiProcessContext(IBaseProcess<VoidResult> process) : base(process)
        {
        }

        /// <summary>
        /// Http context
        /// </summary>
        [Import] public IHttpContextWrapper HttpContext { get; set; }
    }
}