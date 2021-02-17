using App.Domain.Entities.Users;
using App.Domain.Processes.Users;
using App.Domain.Processes.Users.Dtos.Request;
using Signals.Core.Processes;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Authentication;
using Signals.Core.Processing.Authorization;
using Signals.Core.Processing.Results;

namespace App.Client.Web.Processes.Users
{
    [ApiProcess(HttpMethod = ApiProcessMethod.POST)]
    [SignalsAuthenticateProcess]
    [SignalsAuthorizeProcess(UserType.SystemAdministrator)]
    public class DeleteUser : ApiProcess<DeleteUserRequestDto, VoidResult>
    {
        /// <summary>
        /// Auth process
        /// </summary>
        /// <param name="deleteUserRequestDto"></param>
        /// <returns></returns>
        public override VoidResult Auth(DeleteUserRequestDto deleteUserRequestDto)
        {
            return Ok();
        }

        /// <summary>
        /// Validate process
        /// </summary>
        /// <param name="deleteUserRequestDto"></param>
        /// <returns></returns>
        public override VoidResult Validate(DeleteUserRequestDto deleteUserRequestDto)
        {
            return Ok();
        }

        /// <summary>
        /// Handle process
        /// </summary>
        /// <param name="deleteUserRequestDto"></param>
        /// <returns></returns>
        public override VoidResult Handle(DeleteUserRequestDto deleteUserRequestDto)
        {
            var deleteUserResult = Continue<UserProcesses.DeleteUser>()
                                    .With(deleteUserRequestDto);

            return deleteUserResult;
        }
    }
}