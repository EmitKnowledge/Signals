using Signals.Core.Processes.Api;
using Signals.Core.Processing.Results;

namespace Signals.Tests.Core.Web.Web.Api
{
    public class Ping : ApiProcess<MethodResult<bool>>
    {
        /// <summary>
        /// Auth process
        /// </summary>
        /// <returns></returns>
        public override MethodResult<bool> Auth()
        {
            return Ok();
        }

        /// <summary>
        /// Validate process
        /// </summary>
        /// <returns></returns>
        public override MethodResult<bool> Validate()
        {
            return Ok();
        }

        /// <summary>
        /// Handle process
        /// </summary>
        /// <returns></returns>
        public override MethodResult<bool> Handle()
        {
            return true;
        }
    }
}
