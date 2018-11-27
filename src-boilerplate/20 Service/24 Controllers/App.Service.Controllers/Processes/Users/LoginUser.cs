using Signals.Core.Processes.Api;
using Signals.Core.Processing.Results;
using System;

namespace App.Service.Controllers.Processes.Users
{
    public class LoginUser : ApiProcess<VoidResult>
    {
        public override VoidResult Authenticate()
        {
            throw new NotImplementedException();
        }

        public override VoidResult Authorize()
        {
            throw new NotImplementedException();
        }

        public override VoidResult Handle()
        {
            throw new NotImplementedException();
        }

        public override VoidResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}