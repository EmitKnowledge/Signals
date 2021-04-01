using Signals.Clients.WebApi.BusinessProcesses.Dtos.Out;
using Signals.Clients.WebApi.DistributedProcesses.Dtos.In;
using Signals.Clients.WebApi.DistributedProcesses.Dtos.Transient;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Results;

namespace Signals.Clients.WebApi.ApiProcesses
{
    [ApiProcess(HttpMethod = ApiProcessMethod.POST)]
    public class CreateUserWithActivationEmail : ApiDistributedProcess<DistributedProcesses.CreateUser, CreateUserTransientDto, CreateUserDto, MethodResult<UserDto>>
    {
        
    }
}