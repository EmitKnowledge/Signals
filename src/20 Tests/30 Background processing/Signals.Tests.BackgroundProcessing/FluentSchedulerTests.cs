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
        public RecurrencePatternConfiguration MyTaskConfig { get; private set; }

        public FluentSchedulerTests()
        {
            MyTask = new MyFluentTask();
            MyTaskConfig = new TimePartRecurrencePatternConfiguration(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void CustomTask_ExecutedEveryMinute_IsExecutedMultipleTimes()
        {
            lock (MyFluentTask.LockObj)
            {
                MyFluentTask.TimesExecuted = 0;
                var registry = new FluentRegistry();

                registry.ScheduleTask(MyTask, MyTaskConfig);

                registry.Start();
                Thread.Sleep(2100);

                Assert.Equal(2, MyFluentTask.TimesExecuted);

                registry.Stop();
            }
        }
    }
}
