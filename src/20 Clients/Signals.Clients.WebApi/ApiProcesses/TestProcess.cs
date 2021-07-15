using App.Clients.Processes;
using Signals.Core.Processes;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Results;

namespace Signals.Clients.WebApi.ApiProcesses
{
    public class TestProcess : ApiProcess<VoidResult>
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
            return Continue<DistProcess>().With();
        }
    }
}
