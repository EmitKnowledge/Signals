using Signals.Aspects.DI;
using Signals.Aspects.CommunicationChannels;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Input.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processes.Distributed
{
    /// <summary>
    /// Distributed process context
    /// </summary>
    public interface IDistributedProcessContext : IBaseProcessContext
    {
        /// <summary>
        /// Message channel
        /// </summary>
        IMessageChannel Channel { get; }

        /// <summary>
        /// Http context
        /// </summary>
        IHttpContextWrapper HttpContext { get; }
    }

    /// <summary>
    /// Distributed process context
    /// </summary>
    [Export(typeof(IDistributedProcessContext))]
    public class DistributedProcessContext : BaseProcessContext, IDistributedProcessContext
    {
        /// <summary>
        /// Message channel
        /// </summary>
        [Import] public IMessageChannel Channel { get; internal set; }

        /// <summary>
        /// Http context
        /// </summary>
        [Import] public IHttpContextWrapper HttpContext { get; internal set; }
    }
}
