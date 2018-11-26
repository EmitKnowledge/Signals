using Signals.Aspects.ErrorHandling.Strategies;
using Signals.Aspects.ErrorHandling.Polly;
using System;
using System.Linq;
using System.Threading;
using Xunit;

namespace Signals.Tests.ErrorHandling
{
    public class PollyTest
    {
        [Theory]
        [InlineData(true, 10, 3, 10)]
        [InlineData(false, 10, 3, 3)]
        [InlineData(false, 10, 10, 10)]
        public void CircutBreakerPolicy_WhenException_Break(bool isPositive, int executionTimes, int retriesConfiguration, int expectedExecutions)
        {
            var policyRegistry = new StrategyBuilder();
            var timesExecuted = 0;

            var strategy = new CircutBreakerStrategy
            {
                AllowedExceptionsCount = retriesConfiguration,
                CooldownTimeout = TimeSpan.FromMilliseconds(100),
            };

            var handler = policyRegistry.Add<Exception>(strategy).Build();

            for (int i = 0; i < executionTimes; i++)
            {
                var executionResult = handler.Execute(() =>
                    {
                        timesExecuted++;
                        var num = GetNumber(isPositive);
                        if (num <= 0) throw new Exception();
                        return num;
                    });

            }
            Assert.Equal(expectedExecutions, timesExecuted);

            Thread.Sleep(100);

            var executionResult2 = handler.Execute(() =>
            {
                timesExecuted++;
                var num = GetNumber(isPositive);
                if (num <= 0) throw new Exception();
                return num;
            });

            Assert.Equal(++expectedExecutions, timesExecuted);
        }

        public int GetNumber(bool isPositive)
        {
            return isPositive ? 1 : -1;
        }

        [Theory]
        [InlineData(true, false, 3, 1)]
        [InlineData(false, true, 3, 4)]
        [InlineData(false, true, 10, 11)]
        public void RetryPolicy_WhenException_Retry(bool isPositive, bool expectedException, int retriesConfiguration, int expectedExecutions)
        {
            var policyRegistry = new StrategyBuilder();
            var timesExecuted = 0;
            Exception exception = null;

            var strategy = new RetryStrategy {
                RetryCount = retriesConfiguration,
                OnRetry = (ex) => exception = ex
            };

            var policy = policyRegistry.Add<Exception>(strategy).Build();
            var executionResult = policy.Execute(() =>
            {
                timesExecuted++;
                var num = GetNumber(isPositive);
                if (num <= 0) throw new Exception();
                return num;
            });

            Assert.Equal(expectedExecutions, timesExecuted);
            Assert.Equal(expectedException, executionResult.Exception != null);
            Assert.Equal(expectedException, exception != null);
        }
    }
}
