using Signals.Clients.WebApi.BusinessProcesses.Dtos.In;
using Signals.Clients.WebApi.Entities;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;

namespace Signals.Clients.WebApi.BusinessProcesses
{
    public class CreateUser : BusinessProcess<CreateUserDto, MethodResult<User>>
    {
        public override MethodResult<User> Auth(CreateUserDto user)
        {
            return Ok();
        }

        public override MethodResult<User> Validate(CreateUserDto user)
        {
            return Ok();
        }

        public override MethodResult<User> Handle(CreateUserDto user)
        {
            return new User
            {
                Id = 0,
                Email = user.Email
            };
        }
    }
}
