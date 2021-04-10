using Signals.Aspects.DI;
using Signals.Aspects.DI.Attributes;

namespace Signals.Tests.DI.Services.Impl
{
    [Export(typeof(IMyService2))]
    public class MyService2 : IMyService2
    {
        private IMyService3 _myService3 { get; set; }

        public MyService2(IMyService3 myService3)
        {
            _myService3 = myService3;
        }

        public int Subscract(int val1, int val2)
        {
            return (val1 - val2) * (_myService3.GetSign(val1) * _myService3.GetSign(val1));
        }
    }
}
