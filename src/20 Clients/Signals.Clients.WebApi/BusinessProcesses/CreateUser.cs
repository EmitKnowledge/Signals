using Signals.Clients.WebApi.BusinessProcesses.Entities;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;

namespace Signals.Clients.WebApi.BusinessProcesses
{
    public class CreateUser : BusinessProcess<User, MethodResult<User>>
    {
        public override MethodResult<User> Auth(User user)
        {
            return Ok();
        }

        public override MethodResult<User> Validate(User user)
        {
            return Ok();
        }

        public override MethodResult<User> Handle(User user)
        {
            return user;
        }
    }
}
