using Signals.Clients.WebApi.ApiProcesses.Dtos.In;
using Signals.Clients.WebApi.BusinessProcesses.Dtos.In;
using Signals.Core.Processes;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;

namespace Signals.Clients.WebApi.BusinessProcesses
{
    public class ProcessD : BusinessProcess<InputDto, VoidResult>
    {
        public override VoidResult Auth(InputDto input)
        {
            return Ok();
        }

        public override VoidResult Validate(InputDto input)
        {
            return Ok();
        }

        public override VoidResult Handle(InputDto input)
        {
            return Ok();
        }
    }
}
