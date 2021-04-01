using Signals.Clients.WebApi.BusinessProcesses.Dtos.In;
using Signals.Clients.WebApi.BusinessProcesses.Dtos.Out;
using Signals.Core.Processes.Api;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Guards;
using Signals.Core.Processing.Results;
using System.Net;

namespace Signals.Clients.WebApi.ApiProcesses
{
    [ApiProcess(HttpMethod = ApiProcessMethod.POST)]
    [SignalsGuard(typeof(SubscriptionGuard), "Success")]
    [SignalsGuard(typeof(SubscriptionGuard), "Fail")]
    public class CreateUser : ApiBusinessProcess<BusinessProcesses.CreateUser, CreateUserDto, MethodResult<UserDto>>
    {

    }

    public class SubscriptionGuard : ISignalsGuard
    {
        /// <summary>
        /// Checks if the guard is passed
        /// </summary>
        /// <returns></returns>
        public bool Check(IBaseProcessContext processContext, object[] args)
        {
            var arg = args[0].ToString();
            return arg == "Success";
        }

        /// <summary>
        /// Represents the error generated if the guard check fails
        /// </summary>
        public CodeSpecificErrorInfo GetCodeSpecificErrorInfo(object[] args)
        {
            return new CodeSpecificErrorInfo("Error, oh error", HttpStatusCode.Forbidden);
        }
    }
}