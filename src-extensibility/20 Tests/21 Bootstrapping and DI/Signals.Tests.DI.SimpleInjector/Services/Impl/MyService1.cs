using Signals.Aspects.DI;
using Signals.Aspects.DI.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Tests.DI.Services.Impl
{
    [Export(typeof(IMyService1))]
    public class MyService1 : IMyService1
    {
        [Import] public IMyService2 MyService2 { get; set; }

        public int Add(int val1, int val2)
        {
            return val1 + val2 + MyService2.Subscract(val1, val2) - MyService2.Subscract(val1, val2);
        }
    }

    public class MyService1Override : IMyService1
    {
        [Import] public IMyService2 MyService2 { get; set; }

        public int Add(int val1, int val2)
        {
            return val1 + val2 + MyService2.Subscract(val1, val2) - MyService2.Subscract(val1, val2) + 1;
        }
    }
}
