using Ganss.XSS;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Behaviour;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;
using Signals.Core.Processing.Specifications;
using Signals.Core.Web.Behaviour;
using System.IO;

namespace Signals.Clients.NetCoreWeb.BusinessProcesses
{
    public class MySpecContainer : BaseSpecificationContainer<Data>
    {
        public MySpecContainer()
        {
            Add(new MySpec());
            Add(new MySpec2());
        }
    }

    public class MySpec : BaseSpecification<Data>
    {
        public override bool Validate(Data input)
        {
            return true;
        }
    }

    public class MySpec2 : BaseSpecification<Data>
    {
        public override bool Validate(Data input)
        {
            return false;
        }
    }

    [ApiProcess]
    [OutputCache(Duration = 10, Location = CacheLocation.Server, VaryByQueryParams = new string[]{ "age" })]
    [ContentSecurityPolicy]
    [ResponseHeader("custom-header", "my value")]
    public class RegisterUser : ApiProcess<MethodResult<DataResponse>>
    {
        /// <summary>
        /// Authentication layer
        /// </summary>
        /// <returns></returns>
        public override MethodResult<DataResponse> Authenticate()
        {
            return new MethodResult<DataResponse>();
        }

        /// <summary>
        /// Authorization layer
        /// </summary>
        /// <returns></returns>
        public override MethodResult<DataResponse> Authorize()
        {
            return new MethodResult<DataResponse>();
        }

        /// <summary>
        /// Validation layer
        /// </summary>
        /// <returns></returns>
        public override MethodResult<DataResponse> Validate()
        {
            return new MethodResult<DataResponse>();
        }

        /// <summary>
        /// Execution layer
        /// </summary>
        /// <returns></returns>
        public override MethodResult<DataResponse> Handle()
        {
            return new DataResponse
            {
                Number = 10
            };
        }
    }

    public class MyStream : MemoryStream, IDtoData
    {
        public void Sanitize(HtmlSanitizer sanitizer)
        {
        }
    }

    public class RegisterUserEx2 : ApiProcess<Data, MethodResult<MyStream>>
    {
        public override MethodResult<MyStream> Authenticate(Data data)
        {
            return new MyStream();
        }

        public override MethodResult<MyStream> Authorize(Data data)
        {
            return new MyStream();
        }

        public override MethodResult<MyStream> Validate(Data data)
        {
            return new MyStream();
        }

        public override MethodResult<MyStream> Handle(Data data)
        {
            return new MyStream();
        }
    }
}
