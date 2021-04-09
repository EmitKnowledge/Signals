using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;

namespace Signals.Clients.WebApi.BusinessProcesses
{
    public class DoNothing : BusinessProcess<VoidResult>
    {
        public override VoidResult Auth()
        {
            return Ok();
        }

        public override VoidResult Validate()
        {
            return Ok();
        }

        public override VoidResult Handle()
        {
            return Ok();
        }
    }
}
