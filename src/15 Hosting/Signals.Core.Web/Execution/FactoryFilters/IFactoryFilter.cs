using Signals.Core.Processes.Base;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.Core.Web.Execution.FactoryFilters
{
    /// <summary>
    /// Process factory creation handler
    /// </summary>
    public interface IFactoryFilter
    {
        /// <summary>
        /// Validates instance
        /// </summary>
        /// <typeparam name="TProcess"></typeparam>
        /// <param name="instance"></param>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <returns>Is instance valid</returns>
        MiddlewareResult IsValidInstance<TProcess>(TProcess instance, Type type, IHttpContextWrapper context) where TProcess : IBaseProcess<VoidResult>;
    }
}
