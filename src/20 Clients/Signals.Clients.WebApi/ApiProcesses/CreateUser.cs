using Signals.Clients.WebApi.ApiProcesses.Dtos.In;
using Signals.Clients.WebApi.ApiProcesses.Dtos.Out;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Results;

namespace Signals.Clients.WebApi.ApiProcesses
{
    [ApiProcess(HttpMethod = ApiProcessMethod.GET)]
    public class CreateUser : ApiProcess<CreateUserDto, MethodResult<UserDto>>
    {
        public override MethodResult<UserDto> Auth(CreateUserDto createUserDto)
        {
            return Ok();
        }

        public override MethodResult<UserDto> Validate(CreateUserDto createUserDto)
        {
            return Ok();
        }

        public override MethodResult<UserDto> Handle(CreateUserDto createUserDto)
        {
            return Ok();
        }
    }
}
