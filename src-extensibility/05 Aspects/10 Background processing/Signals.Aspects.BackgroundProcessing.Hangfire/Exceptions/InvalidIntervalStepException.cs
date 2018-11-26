using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using System;

namespace Signals.Aspects.BackgroundProcessing.Hangfire.Exceptions
{
    public class InvalidIntervalStepException : Exception
    {
        public InvalidIntervalStepException(SyncTaskConfiguration config)
            : base($"Cannot schedule task if the interval is {config.Interval} and of type {config.IntervalType.ToString()}")
        {
        }
    }
}
