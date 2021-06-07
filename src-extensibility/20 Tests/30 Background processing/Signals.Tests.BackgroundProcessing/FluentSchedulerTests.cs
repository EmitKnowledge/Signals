using Signals.Aspects.BackgroundProcessing;
using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Aspects.BackgroundProcessing.FluentScheduler;
using Signals.Tests.BackgroundProcessing.Tasks;
using System;
using System.Threading;
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
    }
}
