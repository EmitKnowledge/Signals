using Signals.Core.Common.Instance;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Results;
using Signals.Core.Processing.Specifications;
using System;

namespace App.Service.Controllers.Processes.Users
{
    public class LoginUser : ApiProcess<VoidResult>
    {
        public override VoidResult Authenticate()
        {
            return Context.CurrentUserPrincipal.IsNull() ? 
                Fail() : 
                Ok();
        }

        public override VoidResult Authorize()
        {
            return Ok();
        }

        public override VoidResult Validate()
        {
            return Ok();
        }

        public override VoidResult Handle()
        {
            return Ok();
        }
    }
}