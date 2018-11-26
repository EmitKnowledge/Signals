using Signals.Aspects.CommunicationChannels.Base;
using System;

namespace Signals.Aspects.CommunicationChannels.EventGrid
{
    public class EventGridMessageChannel : IMessageChannel
    {
        /// <summary>
        /// Close the subscriber
        /// </summary>
        public void Close()
        {

        }

        /// <summary>
        /// Publish a message
        /// </summary>
        /// <param name="message"></param>
        public void Publish<T>(T message) where T : class
        {

        }

        /// <summary>
        /// Publish a message on a predefined channel
        /// </summary>
        /// <param name="message"></param>
        public void Publish<T>(string channelName, T message) where T : class
        {

        }

        /// <summary>
        /// Subscribe to a message type
        /// </summary>
        /// <param name="action"></param>
        public void Subscribe<T>(Action<T> action) where T : class
        {

        }

        /// <summary>
        /// Subscribe to a message type on a predefined channel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="action"></param>
        public void Subscribe<T>(string channelName, Action<T> action) where T : class
        {

        }
    }
}
