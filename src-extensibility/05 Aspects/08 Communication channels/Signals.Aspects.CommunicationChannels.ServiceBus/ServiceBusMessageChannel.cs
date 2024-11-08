using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using Signals.Aspects.CommunicationChannels.Exceptions;
using Signals.Aspects.CommunicationChannels.ServiceBus.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.Aspects.CommunicationChannels.ServiceBus
{
    /// <summary>
    /// Service bus message channel
    /// </summary>
    public class ServiceBusMessageChannel : IMessageChannel
    {
        private readonly ServiceBusChannelConfiguration _configuration;
        private Dictionary<string, IQueueClient> Subscriptions { get; set; }
        private static readonly HashSet<string> CreatedChannels = new HashSet<string>();

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public ServiceBusMessageChannel(ServiceBusChannelConfiguration configuration)
        {
            _configuration = configuration;
            Subscriptions = new Dictionary<string, IQueueClient>();

        }

        /// <summary>
        /// Close the subscriber
        /// </summary>
        public Task Close()
        {
            Task.WaitAll(Subscriptions.Values.Select(x => x.CloseAsync()).ToArray());

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
            var queueTask = GetQueue(channelName);

            var messageBody = JsonConvert.SerializeObject(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageBody);
            var queueMessage = new ServiceBusMessage(messageBytes);

            var queue = await queueTask;
            if (queue == null) throw new ChannelDoesntExistException(channelName);

            try
            {
                await queue.SendAsync(queueMessage);
            }
            catch (MessagingEntityNotFoundException)
            {
                throw new ChannelDoesntExistException(channelName);
            }
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
        public async Task Subscribe<T>(string channelName, Action<T> action) where T : class
        {
            var queue = await GetQueue(channelName);
            Subscriptions.Add(channelName, queue);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = _configuration.MaxConcurrentCalls,
            };

            // Register the function that processes messages.
            queue.RegisterMessageHandler(async (ServiceBusMessage message, CancellationToken token) =>
            {
                var messageBytes = message.Body;
                var messageBody = Encoding.UTF8.GetString(messageBytes);

                if (messageBody is T)
                {
                    action(messageBody as T);
                }
                else
                {
                    var obj = JsonConvert.DeserializeObject<T>(messageBody);
                    action(obj);
                }

                if (queue.ReceiveMode == ReceiveMode.PeekLock)
                {
                    await queue.CompleteAsync(message.SystemProperties.LockToken);
                }
            }, messageHandlerOptions);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Get service bus queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        private Task<IQueueClient> GetQueue(string queueName)
        {
            var fullQueueName = $"{_configuration.ChannelPrefix}{queueName}";

            if (!CreatedChannels.Contains(fullQueueName))
            {
                lock (CreatedChannels)
                {
                    if (!CreatedChannels.Contains(fullQueueName))
                    {
                        var managementClient = new ManagementClient(_configuration.ConnectionString);
                        var exists = managementClient.QueueExistsAsync(fullQueueName).Result;

                        if (!exists)
                        {
                            managementClient.CreateQueueAsync(fullQueueName).Wait();
                        }
                        CreatedChannels.Add(fullQueueName);
                    }
                }
            }

            var client = new QueueClient(_configuration.ConnectionString, fullQueueName, ServiceBusReceiveMode.PeekLock, RetryPolicy.Default);
            client.OperationTimeout = TimeSpan.FromMinutes(5);

            return Task.FromResult((IQueueClient)client);
        }
    }
}
