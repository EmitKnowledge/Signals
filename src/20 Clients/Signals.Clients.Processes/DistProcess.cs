using Signals.Core.Processes.Distributed;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;

namespace App.Clients.Processes
{
    public class TransientData : ITransientData { }

    public class DistProcess : DistributedProcess<TransientData, VoidResult>
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

        public override VoidResult Work(TransientData request)
        {
            return Ok();
        }

        public override TransientData Map(VoidResult response)
        {
            return new ();
        }
    }
}
