﻿using Signals.Aspects.BackgroundProcessing;
using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Aspects.DI;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Recurring;
using Signals.Core.Processing.Results;
using System;

namespace App.Client.BackgroundServiceWorker.Tasks.Sync.Users
{
    internal class SendTrialEndEmailTask : RecurringProcess<VoidResult>
    {
        [Import] private ISyncLogProvider _provider { get; set; }

        public override RecurrencePatternConfiguration Profile => new TimePartRecurrencePatternConfiguration(TimeSpan.FromSeconds(10));
        
        /// <summary>
        /// Sync data
        /// </summary>
        public override VoidResult Sync()
        {
            var logs = Context.Last(10);
            // do stuff
            return new VoidResult();
        }
    }
}
