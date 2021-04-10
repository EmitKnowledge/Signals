using Signals.Aspects.DI;
using Signals.Aspects.DI.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Tests.DI.Services.Impl
{
    [Export(typeof(IMyService4))]
    public class MyService4 : IMyService4
    {
        public int DoNothing(int val)
        {
            return val;
        }
    }
}
