using Signals.Aspects.Bootstrap;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Business.Base;
using Signals.Core.Processing.Input.Http;

namespace Signals.Core.Business.Api
{
    /// <summary>
    /// Api process context
    /// </summary>
    public class ApiProcessContext : BaseProcessContext
    {
        /// <summary>
        /// Http context
        /// </summary>
        [Import] public IHttpContextWrapper HttpContext { get; set; }
    }
}