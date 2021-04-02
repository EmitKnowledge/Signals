using Signals.Clients.WebApi.ApiProcesses.Dtos.Out;
using Signals.Clients.WebApi.BusinessProcesses.Dtos.In;
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
            return new UserDto
            {
                Id = 0,
                Email = user.Email
            };
        }
    }
}
