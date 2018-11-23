using Signals.Aspects.CommunicationChannels;
using Signals.Aspects.CommunicationChannels.Msmq;
using Signals.Aspects.CommunicationChannels.Msmq.Configurations;
using Signals.Tests.CommunicaitonChannels.Msmq.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Signals.Tests.CommunicaitonChannels.Msmq
{
    public class MsmqTests
    {
        private string Message => "Msg";
        private DateTime Timestamp => new DateTime(2000, 1, 1);
        private MyEvent Event => new MyEvent { Message = Message, Timestamp = Timestamp };

        private async Task<bool> WarpSubscription<T>(
            IMessageChannel channel,
            Action<T> subscription,
            string channelName = null,
            int maxWaitMillisecondTime = 2000) where T : class
        {
            var iterations = 0;
            var waitMillisecondTime = 10;

            return await Task.Run<bool>(() =>
            {
                var hasFired = false;

                if (channelName == null)
                {
                    channel.Subscribe<T>(_event =>
                    {
                        subscription(_event);
                        hasFired = true;
                    });
                }
                else
                {
                    channel.Subscribe<T>(channelName, _event =>
                    {
                        subscription(_event);
                        hasFired = true;
                    });
                }

                while (!hasFired && iterations++ * waitMillisecondTime < maxWaitMillisecondTime) Thread.Sleep(waitMillisecondTime);
                return hasFired;
            });

        }

        [Fact]
        public void Channel_PublishingEvent_IsListened()
        {
            var channel = new MsmqMessageChannel(new MsmqChannelConfiguration());

            var task = WarpSubscription<MyEvent>(channel, _event =>
            {
                Assert.Equal(Message, _event.Message);
                Assert.Equal(Timestamp, _event.Timestamp);
            });

            Thread.Sleep(1000);
            channel.Publish(Event).Wait();

            Assert.True(task.Result);
            channel.Close().Wait();
        }

        [Fact]
        public void Channel_PublishingEventToChannel_IsListened()
        {
            var channelName = "custom_event";
            var channel = new MsmqMessageChannel(new MsmqChannelConfiguration());

            var task = WarpSubscription<MyEvent>(channel, _event =>
            {
                Assert.Equal(Message, _event.Message);
                Assert.Equal(Timestamp, _event.Timestamp);
            }, channelName: channelName);

            Thread.Sleep(1000);
            channel.Publish(channelName, Event).Wait();

            Assert.True(task.Result);
            channel.Close().Wait();
        }

        [Fact]
        public void Channel_PublishingEventToWrongChannel_IsNotListened()
        {
            var channelName1 = "custom_event1";
            var channelName2 = "custom_event2";
            var channel = new MsmqMessageChannel(new MsmqChannelConfiguration());

            var task = WarpSubscription<MyEvent>(channel, _event =>
            {
                Assert.Equal(Message, _event.Message);
                Assert.Equal(Timestamp, _event.Timestamp);
            }, channelName: channelName1);

            Thread.Sleep(1000);

            Assert.Throws<AggregateException>(() =>
            {
                channel.Publish(channelName2, Event).Wait();
            });

            Assert.False(task.Result);
            channel.Close().Wait();
        }

        [Fact]
        public void Channel_PublishingEventClosedChannel_IsNotListened()
        {
            var channel = new MsmqMessageChannel(new MsmqChannelConfiguration());

            var task = WarpSubscription<MyEvent>(channel, _event =>
            {
                Assert.Equal(Message, _event.Message);
                Assert.Equal(Timestamp, _event.Timestamp);
            });

            Thread.Sleep(1000);
            channel.Close().Wait();

            channel.Publish(Event).Wait();

            Assert.False(task.Result);


            var purge = WarpSubscription<MyEvent>(channel, _event =>
            {
            });

            Thread.Sleep(1000);
            Assert.True(purge.Result);
        }
    }
}
