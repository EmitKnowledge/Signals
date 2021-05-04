using Ganss.XSS;
using Signals.Core.Processes;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;
using Signals.Tests.Core.Web.Web.Business;

namespace Signals.Tests.Core.Web.Web.Api
{
    public class AddNumbersDto : IDtoData
    {
        public int A { get; set; }
        public int B { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
        }
    }

    public class AddNumbers : ApiProcess<AddNumbersDto, MethodResult<int>>
    {
        /// <summary>
        /// Auth process
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public override MethodResult<int> Auth(AddNumbersDto dto)
        {
            return Ok();
        }

        /// <summary>
        /// Validate process
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public override MethodResult<int> Validate(AddNumbersDto dto)
        {
            return Ok();
        }

        /// <summary>
        /// Handle process
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public override MethodResult<int> Handle(AddNumbersDto dto)
        {
            ContinueWith<CheckAllAspects, VoidResult>();
            return Continue<AddTwoNumbers>().With(dto.A, dto.B);
        }
    }
}
