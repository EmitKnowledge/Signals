using Ganss.Xss;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;

namespace Signals.Clients.WebApi.ApiProcesses
{
    [SignalsApi(HttpMethod = SignalsApiMethod.POST)]
	public class TestProcess : ApiProcess<CustomerDto, MethodResult<CustomerDto>>
    {
        public override MethodResult<CustomerDto> Auth(CustomerDto dto)
        {
            return Ok();
        }

        public override MethodResult<CustomerDto> Validate(CustomerDto dto)
        {
            return Ok();
        }

        public override MethodResult<CustomerDto> Handle(CustomerDto dto)
        {
            return dto;
        }
    }

    public class CustomerDto : IDtoData
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {

        }
    }
}
