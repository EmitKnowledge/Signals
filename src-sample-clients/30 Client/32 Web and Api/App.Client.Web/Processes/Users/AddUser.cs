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
    public class AddUser : ApiProcess<AddUserRequestDto, VoidResult>
    {
        /// <summary>
        /// Auth process
        /// </summary>
        /// <param name="addUserDto"></param>
        /// <returns></returns>
        public override VoidResult Auth(AddUserRequestDto addUserDto)
        {
            return Ok();
        }

        /// <summary>
        /// Validate process
        /// </summary>
        /// <param name="addUserDto"></param>
        /// <returns></returns>
        public override VoidResult Validate(AddUserRequestDto addUserDto)
        {
            return Ok();
        }

        /// <summary>
        /// Handle process
        /// </summary>
        /// <param name="addUserDto"></param>
        /// <returns></returns>
        public override VoidResult Handle(AddUserRequestDto addUserDto)
        {
            var addUserResult = Continue<UserProcesses.AddUser>()
                                    .With(addUserDto);

            return addUserResult;
        }
    }
}