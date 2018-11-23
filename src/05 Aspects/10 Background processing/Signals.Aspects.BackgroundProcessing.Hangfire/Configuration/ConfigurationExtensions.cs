using Hangfire;
using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Aspects.BackgroundProcessing.Hangfire.Exceptions;
using System;

namespace Signals.Aspects.BackgroundProcessing.Hangfire.Configuration
{
    internal static class ConfigurationExtensions
    {
        internal static void Configure(this BackgroundJobServer server, ISyncTask task, SyncTaskConfiguration configuration)
        {
            var configExpression = "* * * * * *";

            if (configuration.FirstRun.HasValue)
            {
                var firstRunDifference = configuration.FirstRun.Value - DateTime.Now;
                configuration.FirstRun = firstRunDifference.TotalMinutes < 1 ? DateTime.Now.AddMinutes(1) : configuration.FirstRun;

                if (configuration.IntervalType == RecurrancePeriodType.Minute)
                {
                    if (firstRunDifference.TotalMinutes >= 1) throw new InvalidFirstRunDateException(configuration);
                    if (30 % configuration.Interval != 0) throw new InvalidIntervalStepException(configuration);
                    configExpression = configuration.FirstRun.Value.ToString($@"*\/{configuration.Interval} * * * *");
                }
                else if (configuration.IntervalType == RecurrancePeriodType.Hour)
                {
                    if (firstRunDifference.TotalHours >= 1) throw new InvalidFirstRunDateException(configuration);
                    if (12 % configuration.Interval != 0) throw new InvalidIntervalStepException(configuration);
                    configExpression = configuration.FirstRun.Value.ToString($@"m */\/{configuration.Interval} * * *");
                }
                else if (configuration.IntervalType == RecurrancePeriodType.Day)
                {
                    if (firstRunDifference.TotalDays >= 1) throw new InvalidFirstRunDateException(configuration);
                    configExpression = configuration.FirstRun.Value.ToString($@"m H *\/{configuration.Interval} * *");
                }
                else if (configuration.IntervalType == RecurrancePeriodType.Week)
                {
                    if (firstRunDifference.TotalDays >= 7) throw new InvalidFirstRunDateException(configuration);
                    var dayOfWeek = (int)configuration.FirstRun.Value.DayOfWeek;
                    configExpression = configuration.FirstRun.Value.ToString($@"m H *\/{configuration.Interval * 7} * {dayOfWeek}");
                }
                else if (configuration.IntervalType == RecurrancePeriodType.Month)
                {
                    if (firstRunDifference.TotalDays >= 31) throw new InvalidFirstRunDateException(configuration);
                    configExpression = configuration.FirstRun.Value.ToString($@"m H d *\/{configuration.Interval} *");
                }
                else if (configuration.IntervalType == RecurrancePeriodType.Year)
                {
                    if (firstRunDifference.TotalDays >= 365) throw new InvalidFirstRunDateException(configuration);
                    configExpression = configuration.FirstRun.Value.ToString($@"m H d *\/{configuration.Interval * 12} *");
                }
            }
            else
            {
                if (configuration.IntervalType == RecurrancePeriodType.Minute)
                {
                    if (30 % configuration.Interval != 0) throw new InvalidIntervalStepException(configuration);
                    configExpression = DateTime.Now.ToString($@"*\/{configuration.Interval} * * * *");
                }
                else if (configuration.IntervalType == RecurrancePeriodType.Hour)
                {
                    if (12 % configuration.Interval != 0) throw new InvalidIntervalStepException(configuration);
                    configExpression = DateTime.Now.ToString($@"* *\/{configuration.Interval} * * *");
                }
                else if (configuration.IntervalType == RecurrancePeriodType.Day)
                {
                    configExpression = DateTime.Now.ToString($@"* * *\/{configuration.Interval} * *");
                }
                else if (configuration.IntervalType == RecurrancePeriodType.Week)
                {
                    configExpression = DateTime.Now.ToString($@"* * *\/{configuration.Interval * 7} * *");
                }
                else if (configuration.IntervalType == RecurrancePeriodType.Month)
                {
                    configExpression = DateTime.Now.ToString($@"* * * *\/{configuration.Interval} *");
                }
                else if (configuration.IntervalType == RecurrancePeriodType.Year)
                {
                    configExpression = DateTime.Now.ToString($@"* * * *\/{configuration.Interval} * 12 *");
                }
            }

            RecurringJob.AddOrUpdate(() => task.Execute(), configExpression);
        }
    }
}
