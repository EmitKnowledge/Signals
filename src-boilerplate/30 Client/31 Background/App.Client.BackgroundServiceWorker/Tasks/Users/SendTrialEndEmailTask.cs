using App.Service.DomainEntities.Users;
using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Core.Processes.Recurring;
using Signals.Core.Processing.Results;
using System;

namespace App.Client.BackgroundServiceWorker.Tasks.Sync.Users
{
    internal class SendTrialEndEmailTask : RecurringProcess<VoidResult>
    {
        public override RecurrencePatternConfiguration Profile => new TimePartRecurrencePatternConfiguration(TimeSpan.FromSeconds(10));


        /// <summary>
        /// Sync data
        /// </summary>
        public override VoidResult Sync()
        {
            // do stuff
            return new VoidResult();
        }
    }
}
