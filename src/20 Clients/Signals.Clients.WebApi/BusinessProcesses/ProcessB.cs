using Signals.Clients.WebApi.BusinessProcesses.Dtos.In;
using Signals.Core.Processes;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;
using System;

namespace Signals.Clients.WebApi.BusinessProcesses
{
    public class ProcessB : BusinessProcess<InputDto, VoidResult>
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
            Continue<ProcessC>().With(new InputDto
            {
                Data = Guid.NewGuid().ToString()
            });

            Continue<ProcessD>().With(new InputDto
            {
                Data = Guid.NewGuid().ToString()
            });

            return Ok();
        }
    }
}
