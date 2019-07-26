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
    public class DistributedProcessContext : BaseProcessContext
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="process"></param>
        public DistributedProcessContext(IBaseProcess<VoidResult> process) : base(process)
        {
        }

        /// <summary>
        /// Message channel
        /// </summary>
        [Import] public IMessageChannel Channel { get; set; }

        /// <summary>
        /// Http context
        /// </summary>
        [Import] public IHttpContextWrapper HttpContext { get; set; }
    }
}
