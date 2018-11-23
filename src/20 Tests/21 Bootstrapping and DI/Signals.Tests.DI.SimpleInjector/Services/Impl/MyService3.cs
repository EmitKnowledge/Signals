using Signals.Aspects.DI;
using Signals.Aspects.DI.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Tests.DI.Services.Impl
{
    [Export(typeof(IMyService3))]
    public class MyService3 : IMyService3
    {
        [Import] public IMyService4 MyService4 { get; set; }

        public int GetSign(int val)
        {
            if (val == 0) return 1;
            return Math.Abs(val) / val + (MyService4.DoNothing(val) - MyService4.DoNothing(val));
        }
    }
}
