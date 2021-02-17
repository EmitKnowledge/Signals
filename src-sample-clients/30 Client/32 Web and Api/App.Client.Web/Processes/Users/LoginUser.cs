using App.Domain.Processes.Users;
using App.Domain.Processes.Users.Dtos.Request;
using App.Domain.Processes.Users.Dtos.Response;
using Signals.Core.Processes;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Results;

namespace App.Client.Web.Processes.Users
{
    [ApiProcess(HttpMethod = ApiProcessMethod.POST)]
    public class LoginUser : ApiProcess<LoginUserRequestDto, MethodResult<LoginUserResponseDto>>
    {
        /// <summary>
        /// Auth process
        /// </summary>
        /// <param name="loginUserDto"></param>
        /// <returns></returns>
        public override MethodResult<LoginUserResponseDto> Auth(LoginUserRequestDto loginUserDto)
        {
            return Ok();
        }

        /// <summary>
        /// Validate process
        /// </summary>
        /// <param name="loginUserDto"></param>
        /// <returns></returns>
        public override MethodResult<LoginUserResponseDto> Validate(LoginUserRequestDto loginUserDto)
        {
            return Ok();
        }

        /// <summary>
        /// Handle process
        /// </summary>
        /// <param name="loginUserDto"></param>
        /// <returns></returns>
        public override MethodResult<LoginUserResponseDto> Handle(LoginUserRequestDto loginUserDto)
        {
            var loginUserResult = Continue<UserProcesses.LoginUser>()
                                    .With(loginUserDto);

            return loginUserResult;
        }
    }
}