using Signals.Core.Common.Instance;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Results;

namespace App.Domain.Processes.Users
{
    public class LoginUser : ApiProcess<VoidResult>
    {
        public override VoidResult Auth()
        {
            return Context.CurrentUserPrincipal.IsNull() ? 
                Fail() : 
                Ok();
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