﻿using Signals.Clients.WebApi.ApiProcesses.Dtos.Out;
using Signals.Clients.WebApi.Entities;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Results;

namespace Signals.Clients.WebApi.ApiProcesses
{
    [ApiProcess(HttpMethod = ApiProcessMethod.POST)]
    public class CreateUser1 : ProxyApiProcess<BusinessProcesses.CreateUser, BusinessProcesses.Dtos.In.CreateUserDto, MethodResult<User>, Dtos.In.CreateUserDto, MethodResult<UserDto>>
    {
        public override MethodResult<UserDto> MapResponse(MethodResult<User> response)
        {
            return new UserDto
            {
                Email = response.Result.Email,
                Id = response.Result.Id
            };
        }
    }

    [ApiProcess(HttpMethod = ApiProcessMethod.POST)]
    public class CreateUser2 : ProxyApiProcess<BusinessProcesses.CreateUser, MethodResult<User>, MethodResult<UserDto>>
    {
        public override MethodResult<UserDto> MapResponse(MethodResult<User> response)
        {
            return new UserDto
            {
                Email = response.Result.Email,
                Id = response.Result.Id
            };
        }
    }

    [ApiProcess(HttpMethod = ApiProcessMethod.POST)]
    public class CreateUser3 : AutoApiProcess<BusinessProcesses.CreateUser, BusinessProcesses.Dtos.In.CreateUserDto, Dtos.In.CreateUserDto>
    {

    }

    [ApiProcess(HttpMethod = ApiProcessMethod.POST)]
    public class CreateUser4 : AutoApiProcess<BusinessProcesses.CreateUser>
    {

    }
}