using Signals.Aspects.BackgroundProcessing;
using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Aspects.BackgroundProcessing.FluentScheduler;
using Signals.Tests.BackgroundProcessing.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using FluentScheduler;
using Xunit;

namespace Signals.Tests.BackgroundProcessing
{
    public class FluentSchedulerTests
    {
        public MyFluentTask MyTask { get; private set; }

        public FluentSchedulerTests()
        {
            MyTask = new MyFluentTask();
        }

        [Fact]
        public void CustomTask_ExecutedEverySecond_IsExecutedMultipleTimes()
        {
            lock (MyFluentTask.LockObj)
            {
                MyFluentTask.TimesExecuted = 0;
                var registry = new FluentRegistry();

                registry.ScheduleTask(MyTask, new TimePartRecurrencePatternConfiguration(TimeSpan.FromSeconds(1)));

                registry.Start();
                Thread.Sleep(2100);

                Assert.Equal(2, MyFluentTask.TimesExecuted);

                registry.Stop();
            }
        }

        [Fact]
        public void ProviderTask_ExecutedEverySecond_IsExecutedMultipleTimes()
        {
            lock (MyFluentTask.LockObj)
            {
                MyFluentTask.TimesExecuted = 0;
                var registry = new FluentRegistry();

                registry.ScheduleTask(MyTask, new ConfigurableRecurrencePatternConfiguration(() => new TimePartRecurrencePatternConfiguration(TimeSpan.FromSeconds(1))));

                registry.Start();
                Thread.Sleep(2100);

                Assert.Equal(2, MyFluentTask.TimesExecuted);

                registry.Stop();
            }
        }

        [Fact]
        public void CustomTask_ExecutedWeekly_IsExecuted()
        {
            lock (MyFluentTask.LockObj)
            {
                MyFluentTask.TimesExecuted = 0;
                var registry = new FluentRegistry();

                var now = DateTime.Now;
                var day = DateTime.Now.DayOfWeek;
                var time = DateTime.Now.AddMinutes(1);

                registry.ScheduleTask(MyTask, new WeeklyRecurrencePatternConfiguration(0).On(day).At(time.Hour, time.Minute, time.Second));

                registry.Start();
                Thread.Sleep(60100);

                Assert.Equal(1, MyFluentTask.TimesExecuted);

                registry.Stop();
            }
        }

        [Fact]
        public void DailyIntervalOfOne_RegistersWithoutFluentSchedulerDaysException()
        {
            var registry = new FluentRegistry();

            var exception = Record.Exception(() =>
                registry.ScheduleTask(MyTask, new DailyRecurrencePatternConfiguration(1).At(12, 34, 56)));

            Assert.Null(exception);
            Assert.Single(GetSchedules(registry));
        }

        [Fact]
        public void DailyIntervalOfOne_SchedulesConfiguredWallClockTime()
        {
            var registry = new FluentRegistry();
            var configuredTime = DateTime.Now.AddMinutes(2).TimeOfDay;
            configuredTime = new TimeSpan(configuredTime.Hours, configuredTime.Minutes, 37);

            registry.ScheduleTask(
                MyTask,
                new DailyRecurrencePatternConfiguration(1)
                    .At(configuredTime.Hours, configuredTime.Minutes, configuredTime.Seconds));

            registry.Start();
            try
            {
                var nextRun = Assert.Single(GetSchedules(registry)).NextRun;

                Assert.NotNull(nextRun);
                Assert.True(nextRun.Value > DateTime.Now);
                Assert.True(nextRun.Value <= DateTime.Now.AddDays(1));
                Assert.Equal(configuredTime, nextRun.Value.TimeOfDay);
            }
            finally
            {
                registry.Stop();
            }
        }

        [Fact]
        public void DailyIntervalOfOne_WithRunNow_SchedulesImmediateAndDailyExecutions()
        {
            var registry = new FluentRegistry();
            var configuredTime = DateTime.Now.AddMinutes(2).TimeOfDay;
            configuredTime = new TimeSpan(configuredTime.Hours, configuredTime.Minutes, 41);
            var configuration = new DailyRecurrencePatternConfiguration(1)
                .At(configuredTime.Hours, configuredTime.Minutes, configuredTime.Seconds);
            configuration.RunNow = true;

            lock (MyFluentTask.LockObj)
            {
                MyFluentTask.TimesExecuted = 0;
                registry.ScheduleTask(MyTask, configuration);
                registry.Start();

                try
                {
                    Assert.True(SpinWait.SpinUntil(() => MyFluentTask.TimesExecuted == 1, TimeSpan.FromSeconds(2)));
                    var schedules = GetSchedules(registry);
                    Assert.Equal(2, schedules.Count);
                    Assert.Contains(schedules, schedule =>
                        schedule.NextRun.HasValue && schedule.NextRun.Value.TimeOfDay == configuredTime);
                }
                finally
                {
                    registry.Stop();
                }
            }
        }

        [Fact]
        public void DailyIntervalGreaterThanOne_UsesMultiDaySchedule()
        {
            var registry = new FluentRegistry();
            var beforeRegistration = DateTime.Now;
            var configuredTime = DateTime.Now.AddMinutes(2).TimeOfDay;
            configuredTime = new TimeSpan(configuredTime.Hours, configuredTime.Minutes, 0);

            registry.ScheduleTask(
                MyTask,
                new DailyRecurrencePatternConfiguration(2)
                    .At(configuredTime.Hours, configuredTime.Minutes, configuredTime.Seconds));

            registry.Start();
            try
            {
                var nextRun = Assert.Single(GetSchedules(registry)).NextRun;

                Assert.NotNull(nextRun);
                Assert.Equal(beforeRegistration.Date.AddDays(2), nextRun.Value.Date);
                Assert.Equal(configuredTime, nextRun.Value.TimeOfDay);
            }
            finally
            {
                registry.Stop();
            }
        }

        [Fact]
        public void ExistingNonDailySchedules_RegisterWithoutThrowing()
        {
            var registry = new FluentRegistry();
            var configurations = new RecurrencePatternConfiguration[]
            {
                new TimePartRecurrencePatternConfiguration(TimeSpan.FromSeconds(5)),
                new WeeklyRecurrencePatternConfiguration(1).On(DateTime.Now.DayOfWeek).At(12, 34, 0),
                new MonthlyRecurrencePatternConfiguration(1).On(1).At(12, 34, 0),
                new WorkdayRecurrencePatternConfiguration().At(12, 34, 0),
                new WeekendRecurrencePatternConfiguration().At(12, 34, 0)
            };

            var exception = Record.Exception(() =>
            {
                foreach (var configuration in configurations)
                {
                    registry.ScheduleTask(MyTask, configuration);
                }
            });

            Assert.Null(exception);
            Assert.Equal(6, GetSchedules(registry).Count);
        }

        [Fact]
        public void TimePartRunOnceAt_StillSchedulesFirstRunAtConfiguredTime()
        {
            var registry = new FluentRegistry();
            var firstRun = DateTime.Now.AddMinutes(2);
            var configuration = new TimePartRecurrencePatternConfiguration(TimeSpan.FromSeconds(5))
            {
                RunOnceAt = (firstRun.Hour, firstRun.Minute)
            };

            registry.ScheduleTask(MyTask, configuration);

            registry.Start();
            try
            {
                var nextRun = Assert.Single(GetSchedules(registry)).NextRun;
                Assert.NotNull(nextRun);
                Assert.Equal(firstRun.Hour, nextRun.Value.Hour);
                Assert.Equal(firstRun.Minute, nextRun.Value.Minute);
            }
            finally
            {
                registry.Stop();
            }
        }

        [Fact]
        public void BackgroundBootstrap_WithSeveralDailyIntervalOfOneProcesses_CompletesSuccessfully()
        {
            var registry = new FluentRegistry();
            var scheduledTime = DateTime.Now.AddMinutes(2);

            for (var index = 0; index < 5; index++)
            {
                registry.ScheduleTask(
                    new MyFluentTask(),
                    new DailyRecurrencePatternConfiguration(1)
                        .At(scheduledTime.Hour, scheduledTime.Minute, index));
            }

            var exception = Record.Exception(registry.Start);

            try
            {
                Assert.Null(exception);
                Assert.Equal(5, GetSchedules(registry).Count);
            }
            finally
            {
                registry.Stop();
            }
        }

        private static IReadOnlyList<Schedule> GetSchedules(FluentRegistry registry)
        {
            var schedulesField = typeof(FluentRegistry).GetField("_schedules", BindingFlags.Instance | BindingFlags.NonPublic);
            return Assert.IsType<List<Schedule>>(schedulesField?.GetValue(registry));
        }
    }
}
