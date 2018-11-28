using Signals.Aspects.CommunicationChannels.Exceptions;
using Signals.Aspects.CommunicationChannels.Msmq.Configurations;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Threading.Tasks;

namespace Signals.Aspects.CommunicationChannels.Msmq
{
    /// <summary>
    /// MSMQ communication channel implementation
    /// </summary>
    public class MsmqMessageChannel : IMessageChannel
    {
        private readonly MsmqChannelConfiguration _configuration;
        private Dictionary<string, MessageQueue> Subscriptions { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public MsmqMessageChannel(MsmqChannelConfiguration configuration)
        {
            _configuration = configuration;
            Subscriptions = new Dictionary<string, MessageQueue>();
        }

		/// <summary>
		/// Close the subscriber
		/// </summary>
		public Task Close()
		{
			foreach (var subscription in Subscriptions)
			{
				subscription.Value.Close();
				subscription.Value.Dispose();
			}

			Subscriptions.Clear();
			return Task.CompletedTask;
		}

		/// <summary>
		/// Publish a message
		/// </summary>
		/// <param name="message"></param>
		public async Task Publish<T>(T message) where T : class
        {
            var queueForType = message.GetType();
            var queueName = queueForType.Name;
            await Publish(queueName, message);
        }

	    /// <summary>
	    /// Publish a message on a predefined channel
	    /// </summary>
	    /// <param name="channelName"></param>
	    /// <param name="message"></param>
	    public async Task Publish<T>(string channelName, T message) where T : class
        {
			// prepare the path to the channel
			var channelPath = _configuration.ChannelPath.EndsWith("\\") ? _configuration.ChannelPath : $"{_configuration.ChannelPath}\\";
			var queueAddress = $@"{channelPath}{channelName}";

            if (!MessageQueue.Exists(queueAddress)) throw new ChannelDoesntExistException(channelName);

            await Task.Factory.StartNew(() =>
            {
                using (var messageQueue = new MessageQueue(queueAddress, false, false))
                {
                    var queueMessage = new Message(message);
                    queueMessage.Recoverable = true;
                    queueMessage.UseJournalQueue = false;
                    queueMessage.AttachSenderId = false;
                    messageQueue.Formatter = new XmlMessageFormatter(new[] { typeof(T) });
                    messageQueue.Send(message);
                }
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Subscribe to a message type
        /// </summary>
        /// <param name="action"></param>
        public async Task Subscribe<T>(Action<T> action) where T : class
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            // create channel for type
            var queueForType = typeof(T);
            var queueName = queueForType.Name;
            await Subscribe(queueName, action);
        }

        /// <summary>
        /// Subscribe to a message type on a predefined channel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="action"></param>
        public Task Subscribe<T>(string channelName, Action<T> action) where T : class
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (channelName == null) throw new ArgumentNullException(nameof(channelName));

            // create the msmsq binding address
            var queue = InternalSubscribe(channelName, action);
            Subscriptions.Add(channelName, queue);
			return Task.CompletedTask;
        }

        /// <summary>
        /// Represents the internal implementation of subscribe
        /// </summary>
        private MessageQueue InternalSubscribe<T>(string channelName, Action<T> action)
        {
            // create the msmsq binding address
            var queueAddress = $@"{_configuration.ChannelPath}{channelName}";

            if (!MessageQueue.Exists(queueAddress))
            {
                MessageQueue.Create(queueAddress, false);
            }

            var queue = new MessageQueue(queueAddress, false, false);
            queue.Formatter = new XmlMessageFormatter(new[] { typeof(T) });

            queue.ReceiveCompleted += (sender, e) =>
            {
                try
                {
                    var message = queue.EndReceive(e.AsyncResult);

                    if (message != null)
                    {
                        var messageBody = (T)message.Body;
                        action(messageBody);
                    }
                }catch(MessageQueueException)
                {
                }
            };

            queue.BeginReceive();

            return queue;
        }
    }
}
