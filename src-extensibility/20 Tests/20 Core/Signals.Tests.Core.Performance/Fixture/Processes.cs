using Ganss.XSS;
using Signals.Core.Processes.Api;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;

namespace Signals.Tests.Core.Performance.Fixture
{
    public class SimpleProcess1 : BusinessProcess<Input, MethodResult<int>>
    {
        public override MethodResult<int> Auth(Input request) => Ok();
        public override MethodResult<int> Validate(Input request) => Ok();

        public override MethodResult<int> Handle(Input request)
        {
            return request.Input1 + request.Input2;
        }
    }

    public class SimpleProcess2 : BusinessProcess<Input, MethodResult<int>>
    {
        public override MethodResult<int> Auth(Input request) => Ok();
        public override MethodResult<int> Validate(Input request) => Ok();

        public override MethodResult<int> Handle(Input request)
        {
            return request.Input1 + request.Input2;
        }
    }

    public class WebProcess1 : AutoApiProcess<SimpleProcess1, Input> { }
    public class WebProcess2 : AutoApiProcess<SimpleProcess1, Input> { }
    public class WebProcess3 : AutoApiProcess<SimpleProcess1, Input> { }
    public class WebProcess4 : AutoApiProcess<SimpleProcess1, Input> { }
    public class WebProcess5 : AutoApiProcess<SimpleProcess1, Input> { }
    public class WebProcess6 : AutoApiProcess<SimpleProcess1, Input> { }
    public class WebProcess7 : AutoApiProcess<SimpleProcess1, Input> { }
    public class WebProcess8 : AutoApiProcess<SimpleProcess1, Input> { }
    public class WebProcess9 : AutoApiProcess<SimpleProcess1, Input> { }
    public class WebProcess10 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess11 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess12 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess13 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess14 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess15 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess16 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess17 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess18 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess19 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess20 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess21 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess22 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess23 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess24 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess25 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess26 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess27 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess28 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess29 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess30 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess31 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess32 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess33 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess34 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess35 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess36 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess37 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess38 : AutoApiProcess<SimpleProcess2, Input> { }
    public class WebProcess39 : AutoApiProcess<SimpleProcess2, Input> { }

    public class WebProcess : ApiProcess<Input, MethodResult<int>>
    {
        public override MethodResult<int> Auth(Input request)
        {
            return Ok();
        }

        public override MethodResult<int> Validate(Input request)
        {
            return Ok();
        }

        public override MethodResult<int> Handle(Input request)
        {
            return request?.Input1 ?? 0 + request?.Input2 ?? 0;
        }
    }

    public class Input : IDtoData
    {
        public int Input1 { get; set; }
        public int Input2 { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
        }
    }
}
