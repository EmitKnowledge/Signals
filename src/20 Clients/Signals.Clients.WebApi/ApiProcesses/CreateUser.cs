using Signals.Clients.WebApi.BusinessProcesses.Dtos.In;
using Signals.Clients.WebApi.BusinessProcesses.Dtos.Out;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Guards;
using Signals.Core.Processing.Results;
using System.Net;
using Signals.Core.Processes.Base;

namespace Signals.Clients.WebApi.ApiProcesses
{
    [ApiProcess(HttpMethod = ApiProcessMethod.POST)]
    [SignalsGuards(typeof(SubscriptionGuard))]
    public class CreateUser : ApiBusinessProcess<BusinessProcesses.CreateUser, CreateUserDto, MethodResult<UserDto>>
    {

    }

    public class SubscriptionGuard : ISignalsGuard
    {
        /// <summary>
        /// Checks if the guard is passed
        /// </summary>
        /// <returns></returns>
        public bool Check(IBaseProcessContext processContext)
        {
            return false;
        }

        /// <summary>
        /// Represents the error generated if the guard check fails
        /// </summary>
        public CodeSpecificErrorInfo GetCodeSpecificErrorInfo()
        {
            return new CodeSpecificErrorInfo("Error", HttpStatusCode.PaymentRequired);
        }
    }
}