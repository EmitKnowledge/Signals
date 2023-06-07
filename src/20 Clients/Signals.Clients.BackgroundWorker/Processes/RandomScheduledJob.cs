using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Core.Processes.Recurring;
using Signals.Core.Processing.Results;
using System;

namespace App.Clients.BackgroundWorker.Processes
{
    public class RandomScheduledJob : NoOverlapRecurringProcess<VoidResult>
    {
        /// <summary>
        /// Recurring profile
        /// </summary>
        public override RecurrencePatternConfiguration Profile =>
            new TimePartRecurrencePatternConfiguration(TimeSpan.FromSeconds(10))
            {
                RunOnceAt = (12, 47)
            };

        /// <summary>
        /// Checks if the recurring process should execute
        /// </summary>
        /// <returns></returns>
        public override bool ShouldExecute()
        {
            return false;
        }

        /// <summary>
        /// Background execution layer
        /// </summary>
        /// <returns></returns>
        public override VoidResult Sync()
        {
            return Ok();
        }
    }
}
