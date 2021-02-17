using App.Domain.Processes.Users;
using App.Domain.Processes.Users.Dtos.Request;
using Signals.Core.Processes;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Authentication;
using Signals.Core.Processing.Results;
using System.Collections.Generic;

namespace App.Client.Web.Processes.Users
{
    [ApiProcess(HttpMethod = ApiProcessMethod.POST)]
    [SignalsAuthenticateProcess]
    public class LogoutUser : ApiProcess<VoidResult>
    {
        private string Token => Context.HttpContext.Headers?.GetFromRequest()?.GetValueOrDefault("auth-token")?.ToString();

        /// <summary>
        /// Auth proccess
        /// </summary>
        /// <returns></returns>
        public override VoidResult Auth()
        {
            return Ok();
        }

        /// <summary>
        /// Validate proccess
        /// </summary>
        /// <returns></returns>
        public override VoidResult Validate()
        {
            return Ok();
        }

        /// <summary>
        /// Handle proccess
        /// </summary>
        /// <returns></returns>
        public override VoidResult Handle()
        {
            LogoutUserRequestDto logoutUserRequestDto = new LogoutUserRequestDto { Token = Token };
            var logoutUserResult = Continue<UserProcesses.LogoutUser>()
                                    .With(logoutUserRequestDto);

            return logoutUserResult;
        }
    }
}