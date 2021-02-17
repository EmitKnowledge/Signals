using App.Domain.Processes.Users;
using App.Domain.Processes.Users.Dtos.Request;
using Signals.Core.Processes;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Authentication;
using Signals.Core.Processing.Results;

namespace App.Client.Web.Processes.Users
{
    [ApiProcess(HttpMethod = ApiProcessMethod.POST)]
    [SignalsAuthenticateProcess]
    public class ChangePassword : ApiProcess<ChangePasswordRequestDto, VoidResult>
    {
        /// <summary>
        /// Auth process
        /// </summary>
        /// <param name="changePasswordRequestDto"></param>
        /// <returns></returns>
        public override VoidResult Auth(ChangePasswordRequestDto changePasswordRequestDto)
        {
            return Ok();
        }

        /// <summary>
        /// Validation process
        /// </summary>
        /// <param name="changePasswordRequestDto"></param>
        /// <returns></returns>
        public override VoidResult Validate(ChangePasswordRequestDto changePasswordRequestDto)
        {
            return Ok();
        }

        /// <summary>
        /// Handle process
        /// </summary>
        /// <param name="changePasswordRequestDto"></param>
        /// <returns></returns>
        public override VoidResult Handle(ChangePasswordRequestDto changePasswordRequestDto)
        {
            var changePasswordResult = Continue<UserProcesses.ChangePassword>()
                                    .With(changePasswordRequestDto);

            return changePasswordResult;
        }
    }
}