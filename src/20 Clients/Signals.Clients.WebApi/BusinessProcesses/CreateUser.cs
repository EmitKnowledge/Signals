using Signals.Clients.WebApi.BusinessProcesses.Dtos.In;
using Signals.Clients.WebApi.BusinessProcesses.Dtos.Out;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;

namespace Signals.Clients.WebApi.BusinessProcesses
{
    public class CreateUser : BusinessProcess<CreateUserDto, MethodResult<UserDto>>
    {
        public override MethodResult<UserDto> Auth(CreateUserDto user)
        {
            return Ok();
        }

        public override MethodResult<UserDto> Validate(CreateUserDto user)
        {
            return Ok();
        }

        public override MethodResult<UserDto> Handle(CreateUserDto user)
        {
            return new MethodResult<UserDto>();
        }
    }
}
