using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processes.Business
{
    /// <summary>
    /// Business process context
    /// </summary>
    public interface IBusinessProcessContext : IBaseProcessContext
    {
    }

    /// <summary>
    /// Business process context
    /// </summary>
    [Export(typeof(IBusinessProcessContext))]
    public class BusinessProcessContext : BaseProcessContext, IBusinessProcessContext
    {
    }
}