using Signals.Aspects.BackgroundProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Tests.BackgroundProcessing.Tasks
{
    public class MyHangfireTask : ISyncTask
    {
        public static object LockObj = new object();
        public static int TimesExecuted { get; set; }

        public void Execute()
        {
            TimesExecuted++;
        }
    }
}
