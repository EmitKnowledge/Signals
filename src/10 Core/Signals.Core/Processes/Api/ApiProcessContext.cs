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
    public interface IApiProcessContext : IBaseProcessContext
    {
        /// <summary>
        /// Http context
        /// </summary>
        IHttpContextWrapper HttpContext { get; set; }
    }

    /// <summary>
    /// Api process context
    /// </summary>
    [Export(typeof(IApiProcessContext))]
    public class ApiProcessContext : BaseProcessContext, IApiProcessContext
    {
        /// <summary>
        /// Http context
        /// </summary>
        [Import] public IHttpContextWrapper HttpContext { get; set; }
    }
}