using App.Domain.Processes.Users;
using App.Domain.Processes.Users.Dtos.Request;
using Signals.Core.Processes;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Results;

namespace App.Client.Web.Processes.Users
{
    [ApiProcess(HttpMethod = ApiProcessMethod.POST)]
    public class ResetPassword : ApiProcess<ResetPasswordRequestDto, VoidResult>
    {
        /// <summary>
        /// Auth process
        /// </summary>
        /// <param name="resetPasswordRequestDto"></param>
        /// <returns></returns>
        public override VoidResult Auth(ResetPasswordRequestDto resetPasswordRequestDto)
        {
            return Ok();
        }

        /// <summary>
        /// Validation process
        /// </summary>
        /// <param name="resetPasswordRequestDto"></param>
        /// <returns></returns>
        public override VoidResult Validate(ResetPasswordRequestDto resetPasswordRequestDto)
        {
            return Ok();
        }

        /// <summary>
        /// Handle process
        /// </summary>
        /// <param name="resetPasswordRequestDto"></param>
        /// <returns></returns>
        public override VoidResult Handle(ResetPasswordRequestDto resetPasswordRequestDto)
        {
            var changePasswordResult = Continue<UserProcesses.ResetPassword>()
                                        .With(resetPasswordRequestDto);

            return changePasswordResult;
        }
    }
}