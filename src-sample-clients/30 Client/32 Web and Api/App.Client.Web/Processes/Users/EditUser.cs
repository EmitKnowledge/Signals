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
    public class EditUser : ApiProcess<EditUserRequestDto, VoidResult>
    {
        /// <summary>
        /// Auth process
        /// </summary>
        /// <param name="editUserRequestDto"></param>
        /// <returns></returns>
        public override VoidResult Auth(EditUserRequestDto editUserRequestDto)
        {
            return Ok();
        }

        /// <summary>
        /// Validate process
        /// </summary>
        /// <param name="editUserRequestDto"></param>
        /// <returns></returns>
        public override VoidResult Validate(EditUserRequestDto editUserRequestDto)
        {
            return Ok();
        }

        /// <summary>
        /// Handle process
        /// </summary>
        /// <param name="editUserRequestDto"></param>
        /// <returns></returns>
        public override VoidResult Handle(EditUserRequestDto editUserRequestDto)
        {
            var editUserResult = Continue<UserProcesses.EditUser>()
                .With(editUserRequestDto);

            return editUserResult;
        }
    }
}