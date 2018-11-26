using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using System;

namespace Signals.Aspects.BackgroundProcessing.Hangfire.Exceptions
{
    public class InvalidFirstRunDateException : Exception
    {
        public InvalidFirstRunDateException(SyncTaskConfiguration config)
            : base($"Cannot schedule task for {config.FirstRun?.ToString()} if the interval is of type {config.IntervalType.ToString()}")
        {
        }
    }
}
