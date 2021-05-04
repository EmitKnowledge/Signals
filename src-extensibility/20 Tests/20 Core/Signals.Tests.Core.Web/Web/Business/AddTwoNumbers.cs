using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;

namespace Signals.Tests.Core.Web.Web.Business
{
    public class AddTwoNumbers : BusinessProcess<int, int, MethodResult<int>>
    {
        /// <summary>
        /// Auth process
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public override MethodResult<int> Auth(int a, int b)
        {
            return Ok();
        }

        /// <summary>
        /// Validate process
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public override MethodResult<int> Validate(int a, int b)
        {
            return Ok();
        }

        /// <summary>
        /// Handle process
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public override MethodResult<int> Handle(int a, int b)
        {
            return a + b;
        }
    }
}
