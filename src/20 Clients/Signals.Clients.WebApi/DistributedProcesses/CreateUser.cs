using Signals.Clients.WebApi.BusinessProcesses.Dtos.Out;
using Signals.Clients.WebApi.DistributedProcesses.Dtos.In;
using Signals.Clients.WebApi.DistributedProcesses.Dtos.Transient;
using Signals.Core.Processes.Distributed;
using Signals.Core.Processing.Results;

namespace Signals.Clients.WebApi.DistributedProcesses
{
    public class CreateUser : DistributedProcess<CreateUserTransientDto, CreateUserDto, MethodResult<UserDto>>
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
            return new UserDto
            {
                Email = createUserDto.Email
            };
        }

        public override VoidResult Work(CreateUserTransientDto request)
        {
            throw new System.NotImplementedException();
        }

        public override CreateUserTransientDto Map(CreateUserDto request, MethodResult<UserDto> response)
        {
            return new CreateUserTransientDto();
        }
    }
}
