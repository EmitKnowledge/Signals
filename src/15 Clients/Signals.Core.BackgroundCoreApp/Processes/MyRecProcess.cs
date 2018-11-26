using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Core.Processes.Recurring;
using Signals.Core.Processing.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Core.BackgroundCoreApp.Processes
{
    public class MyRecProcess : RecurringProcess<VoidResult>
    {
        public override RecurrencePatternConfiguration Profile => new TimePartRecurrencePatternConfiguration(TimeSpan.FromSeconds(1));

        public override VoidResult Sync()
        {
            throw new NotImplementedException();
        }
    }
}
