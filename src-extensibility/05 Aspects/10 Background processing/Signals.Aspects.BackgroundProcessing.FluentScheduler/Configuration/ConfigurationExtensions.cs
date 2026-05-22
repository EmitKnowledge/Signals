using FluentScheduler;
using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using System;
using System.Collections.Generic;

namespace Signals.Aspects.BackgroundProcessing.FluentScheduler.Configuration
{
    internal static class ConfigurationExtensions
    {
        internal static void Configure(this ICollection<Schedule> schedules, ISyncTask task, RecurrencePatternConfiguration configuration)
        {
            if (configuration.RunNow)
            {
                schedules.Add(CreateSchedule(task, schedule => schedule.Now()));
            }

            switch (configuration.GetInstance())
            {
                case DailyRecurrencePatternConfiguration dailyConfiguration:
                    schedules.Add(CreateSchedule(task, schedule => ConfigureDaily(schedule, dailyConfiguration)));
                    break;
                case MonthlyNamedRecurrencePatternConfiguration monthlyNamedConfiguration:
                    schedules.Add(CreateMonthlyNamedSchedule(task, monthlyNamedConfiguration));
                    break;
                case MonthlyRecurrencePatternConfiguration monthlyConfiguration:
                    schedules.Add(CreateSchedule(task, schedule => ConfigureMonthly(schedule, monthlyConfiguration)));
                    break;
                case TimePartRecurrencePatternConfiguration timePartConfiguration:
                    schedules.Add(CreateSchedule(task, schedule => ConfigureTimePart(schedule, timePartConfiguration)));
                    break;
                case WeekendRecurrencePatternConfiguration weekendConfiguration:
                    foreach (var day in weekendConfiguration.Days)
                    {
                        schedules.Add(CreateWeeklySchedule(task, day, weekendConfiguration.Value, weekendConfiguration.TimePart));
                    }
                    break;
                case WeeklyRecurrencePatternConfiguration weeklyConfiguration:
                    schedules.Add(CreateWeeklySchedule(task, weeklyConfiguration.Day, weeklyConfiguration.Value, weeklyConfiguration.TimePart));
                    break;
                case WorkdayRecurrencePatternConfiguration workdayConfiguration:
                    schedules.Add(CreateSchedule(task, schedule => ConfigureWorkday(schedule, workdayConfiguration)));
                    break;
            }
        }

        private static Schedule CreateSchedule(ISyncTask task, Action<RunSpecifier> configure)
        {
            return new Schedule(() => task.Execute(), configure);
        }

        private static Schedule CreateGuardedSchedule(ISyncTask task, Func<DateTime, bool> shouldRun, Action<RunSpecifier> configure)
        {
            return new Schedule(
                () =>
                {
                    if (shouldRun(DateTime.Now))
                    {
                        task.Execute();
                    }
                },
                configure);
        }

        private static void ConfigureDaily(RunSpecifier schedule, DailyRecurrencePatternConfiguration configuration)
        {
            schedule
                .Every(configuration.Value)
                .Days()
                .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
        }

        private static Schedule CreateMonthlyNamedSchedule(ISyncTask task, MonthlyNamedRecurrencePatternConfiguration configuration)
        {
            var anchorDate = DateTime.Now.Date;

            return CreateGuardedSchedule(
                task,
                currentDate => IsMonthlyNamedRun(currentDate.Date, anchorDate, configuration),
                schedule => schedule
                    .Every(configuration.Day)
                    .At(configuration.TimePart.Hours, configuration.TimePart.Minutes));
        }

        private static void ConfigureMonthly(RunSpecifier schedule, MonthlyRecurrencePatternConfiguration configuration)
        {
            schedule
                .Every(configuration.Value)
                .Months()
                .On(configuration.Day)
                .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
        }

        private static void ConfigureTimePart(RunSpecifier schedule, TimePartRecurrencePatternConfiguration configuration)
        {
            if (configuration.RunOnceAt.HasValue && !configuration.RunNow)
            {
                var hour = configuration.RunOnceAt.Value.Item1;
                var minute = configuration.RunOnceAt.Value.Item2;

                schedule
                    .OnceAt(hour, minute)
                    .AndEvery(configuration.Value)
                    .Seconds();
            }
            else
            {
                schedule
                    .Every(configuration.Value)
                    .Seconds();
            }
        }

        private static Schedule CreateWeeklySchedule(ISyncTask task, DayOfWeek day, int interval, TimeSpan timePart)
        {
            var anchorDate = DateTime.Now.Date;

            return CreateGuardedSchedule(
                task,
                currentDate => currentDate.DayOfWeek == day && IsWeekIntervalRun(currentDate.Date, anchorDate, interval),
                schedule => schedule
                    .Everyday()
                    .At(timePart.Hours, timePart.Minutes));
        }

        private static void ConfigureWorkday(RunSpecifier schedule, WorkdayRecurrencePatternConfiguration configuration)
        {
            schedule
                .EveryWeekday()
                .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
        }

        private static bool IsWeekIntervalRun(DateTime currentDate, DateTime anchorDate, int interval)
        {
            if (interval <= 1)
            {
                return true;
            }

            var weeks = (int)Math.Floor((currentDate - anchorDate).TotalDays / 7);
            return weeks >= 0 && weeks % interval == 0;
        }

        private static bool IsMonthlyNamedRun(
            DateTime currentDate,
            DateTime anchorDate,
            MonthlyNamedRecurrencePatternConfiguration configuration)
        {
            return currentDate.DayOfWeek == configuration.Day
                   && IsMonthIntervalRun(currentDate, anchorDate, configuration.Value)
                   && IsDayOrderInMonth(currentDate, configuration.Order);
        }

        private static bool IsMonthIntervalRun(DateTime currentDate, DateTime anchorDate, int interval)
        {
            if (interval <= 1)
            {
                return true;
            }

            var months = ((currentDate.Year - anchorDate.Year) * 12) + currentDate.Month - anchorDate.Month;
            return months >= 0 && months % interval == 0;
        }

        private static bool IsDayOrderInMonth(DateTime date, MonthlyNamedRecurrencePatternConfiguration.DayInMonth order)
        {
            switch (order)
            {
                case MonthlyNamedRecurrencePatternConfiguration.DayInMonth.First:
                    return date.Day <= 7;
                case MonthlyNamedRecurrencePatternConfiguration.DayInMonth.Second:
                    return date.Day >= 8 && date.Day <= 14;
                case MonthlyNamedRecurrencePatternConfiguration.DayInMonth.Third:
                    return date.Day >= 15 && date.Day <= 21;
                case MonthlyNamedRecurrencePatternConfiguration.DayInMonth.Fourth:
                    return date.Day >= 22 && date.Day <= 28;
                case MonthlyNamedRecurrencePatternConfiguration.DayInMonth.Last:
                    return date.AddDays(7).Month != date.Month;
                default:
                    return false;
            }
        }
    }
}
