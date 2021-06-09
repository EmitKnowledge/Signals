using Signals.Clients.WebApi.BusinessProcesses.Dtos.In;
using Signals.Core.Processes;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;
using System;

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
            return Fail();
            //Continue<ProcessA>().With(new InputDto
            //{
            //    Data = Guid.NewGuid().ToString()
            //});
            //return Ok();
        }
    }
}
