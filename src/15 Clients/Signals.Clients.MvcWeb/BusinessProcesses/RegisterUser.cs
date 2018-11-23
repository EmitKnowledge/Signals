using Signals.Core.Business.Api;
using Signals.Core.Business.Business;
using Signals.Core.Processing.Authorization;
using Signals.Core.Processing.Results;
using Signals.Core.Processing.Specifications;
using Signals.Core.Web.Behaviour;
using System;

namespace Signals.Clients.MvcWeb.BusinessProcesses
{
    [ApiProcess]
    [OutputCache(Duration = 10, Location = CacheLocation.Server, VaryByQueryParams = new string[] { "age" })]
    [ContentSecurityPolicy]
    [ResponseHeader("custom-header", "my value")]
    public class RegisterUser : ApiProcess<MethodResult<Data>>
    {
        public override MethodResult<Data> Authenticate()
        {
            return new Data();
        }

        public override MethodResult<Data> Authorize()
        {
            return new Data();
        }

        public override MethodResult<Data> Validate()
        {
            return new Data();
        }

        public override MethodResult<Data> Handle()
        {
            var data = new Data();
            return data;
        }
    }
}
