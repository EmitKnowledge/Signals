using App.Domain.DataRepositoryContracts;
using App.Domain.Processes.Users.Dtos.Request;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;

namespace App.Domain.Processes.Users
{
    public partial class UserProcesses
    {
        public class LogoutUser : BusinessProcess<LogoutUserRequestDto, VoidResult>
        {
            [Import] private IUserRepository UserRepository { get; set; }

            /// <summary>
            /// Auth proccess
            /// </summary>
            /// <returns></returns>
            public override VoidResult Auth(LogoutUserRequestDto logoutUserRequestDto)
            {
                return Ok();
            }

            /// <summary>
            /// Validate proccess
            /// </summary>
            /// <returns></returns>
            public override VoidResult Validate(LogoutUserRequestDto logoutUserRequestDto)
            {
                return Ok();
            }

            /// <summary>
            /// Handle proccess
            /// </summary>
            /// <returns></returns>
            public override VoidResult Handle(LogoutUserRequestDto logoutUserRequestDto)
            {
                //var user = UserRepository.GetByToken(logoutUserRequestDto.Token);
                //if (user.IsNull())
                //    return Fail(new GeneralErrorInfo("LoginUser_UserNotFound"));

                //var token = string.Empty;
                //UserRepository.UpdateUserToken(user.Id, token);

                return Ok();
            }
        }
    }
}