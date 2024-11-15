﻿using Signals.Aspects.CommunicationChannels;
using Signals.Aspects.CommunicationChannels.ServiceBus;
using Signals.Aspects.CommunicationChannels.ServiceBus.Configurations;
using Signals.Tests.CommunicaitonChannels.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using Signals.Tests.Configuration;
using Xunit;

namespace Signals.Tests.CommunicaitonChannels
{
    public class ServiceBusTests
    {
        private static BaseTestConfiguration _configuration = BaseTestConfiguration.Instance;
        private ServiceBusChannelConfiguration Configuration => new ServiceBusChannelConfiguration
        {
            ConnectionString = _configuration.ServiceBusConfiguration.ConnectionString
        };

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
            var channel = new ServiceBusMessageChannel(Configuration);

            var task = WarpSubscription<MyEvent>(channel, _event =>
            {
                Assert.Equal(Message, _event.Message);
                Assert.Equal(Timestamp, _event.Timestamp);
            });

            Thread.Sleep(5000);
            channel.Publish(Event).Wait();
            Thread.Sleep(5000);

            Assert.True(task.Result);
            channel.Close().Wait();
        }

        [Fact]
        public void Channel_PublishingEventToChannel_IsListened()
        {
            var channelName = "custom_event";
            var channel = new ServiceBusMessageChannel(Configuration);

            var task = WarpSubscription<MyEvent>(channel, _event =>
            {
                Assert.Equal(Message, _event.Message);
                Assert.Equal(Timestamp, _event.Timestamp);
            }, channelName: channelName);

            Thread.Sleep(1000);
            channel.Publish(channelName, Event).Wait();
            Thread.Sleep(1000);

            Assert.True(task.Result);
            channel.Close().Wait();
        }

        [Fact]
        public void Channel_PublishingEventToWrongChannel_IsNotListened()
        {
            var channelName1 = "custom_sb_event1";
            var channelName2 = "custom_sb_event2";
            var channel = new ServiceBusMessageChannel(Configuration);

            var task = WarpSubscription<MyEvent>(channel, _event =>
            {
                Assert.Equal(Message, _event.Message);
                Assert.Equal(Timestamp, _event.Timestamp);
            }, channelName: channelName1);

            Thread.Sleep(1000);
            channel.Publish(channelName2, Event).Wait();

            Thread.Sleep(1000);

            Assert.False(task.Result);
            channel.Close().Wait();
        }

        [Fact]
        public void Channel_PublishingEventClosedChannel_IsNotListened()
        {
            var channelName = "custom_closed_event";
            var channel = new ServiceBusMessageChannel(Configuration);

            var task = WarpSubscription<MyEvent>(channel, _event =>
            {
                Assert.Equal(Message, _event.Message);
                Assert.Equal(Timestamp, _event.Timestamp);
            }, channelName: channelName);

            Thread.Sleep(1000);
            channel.Close().Wait();

            channel.Publish(channelName, Event).Wait();
            Thread.Sleep(1000);

            Assert.False(task.Result);


            var purge = WarpSubscription<MyEvent>(channel, _event =>
            {
            }, channelName: channelName);

            Thread.Sleep(1000);
            Assert.True(purge.Result);
        }
    }
}
