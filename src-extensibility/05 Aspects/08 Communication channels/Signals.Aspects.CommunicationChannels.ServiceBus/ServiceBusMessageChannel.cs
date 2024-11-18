using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Newtonsoft.Json;
using Signals.Aspects.CommunicationChannels.Exceptions;
using Signals.Aspects.CommunicationChannels.ServiceBus.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.Aspects.CommunicationChannels.ServiceBus
{
	/// <summary>
	/// Service bus message channel
	/// </summary>
	public class ServiceBusMessageChannel : IMessageChannel
	{
		private readonly ServiceBusChannelConfiguration _configuration;
		private Dictionary<string, ServiceBusProcessor> Subscriptions { get; set; }
		private static readonly HashSet<string> CreatedChannels = new HashSet<string>();

		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="configuration"></param>
		public ServiceBusMessageChannel(ServiceBusChannelConfiguration configuration)
		{
			_configuration = configuration;
			Subscriptions = new Dictionary<string, ServiceBusProcessor>();
		}

		/// <summary>
		/// Close the subscriber
		/// </summary>
		public Task Close()
		{
			Task.WaitAll(Subscriptions.Values.Select(x => x.StopProcessingAsync()).ToArray());
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
			await EnsureQueue(channelName);

			// create client
			ServiceBusClient client = new(_configuration.ConnectionString);
			ServiceBusSender sender = client.CreateSender(channelName);

			var messageBody = JsonConvert.SerializeObject(message);
			var messageBytes = Encoding.UTF8.GetBytes(messageBody);
			var queueMessage = new ServiceBusMessage(messageBytes);

			if (sender == null) throw new ChannelDoesntExistException(channelName);

			try
			{
				await sender.SendMessageAsync(queueMessage);
			}
			catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
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

			queue.ProcessErrorAsync += async (ProcessErrorEventArgs args) =>
			{
				Console.WriteLine($"Message handler encountered an exception {args.Exception}.");
				Console.WriteLine("Exception context for troubleshooting:");
				Console.WriteLine($"- FQNS: {args.FullyQualifiedNamespace}");
				Console.WriteLine($"- Entity Path: {args.EntityPath}");
				await Task.CompletedTask;
			};

			// Register the function that processes messages.
			queue.ProcessMessageAsync += async (arg) =>
			{
				var messageBytes = arg.Message.Body.ToArray();
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

				if (queue.ReceiveMode == ServiceBusReceiveMode.PeekLock)
				{
					await arg.CompleteMessageAsync(arg.Message);
				}
			};

			await queue.StartProcessingAsync();
		}

		/// <summary>
		/// Ensure that the pub/sub channel exists before receving or sending messages to it
		/// </summary>
		/// <param name="queueName"></param>
		/// <returns></returns>
		private Task EnsureQueue(string queueName)
		{
			if (!CreatedChannels.Contains(queueName))
			{
				lock (CreatedChannels)
				{
					if (!CreatedChannels.Contains(queueName))
					{
						var fullQueueName = $"{_configuration.ChannelPrefix}{queueName}";
						ServiceBusAdministrationClient managementClient = new(_configuration.ConnectionString);
						var exists = managementClient.QueueExistsAsync(fullQueueName).Result;

						if (!exists)
						{
							managementClient.CreateQueueAsync(fullQueueName).Wait();
						}
						CreatedChannels.Add(queueName);
					}
				}
			}
			return Task.CompletedTask;
		}

		/// <summary>
		/// Get service bus queue
		/// </summary>
		/// <param name="queueName"></param>
		/// <returns></returns>
		private Task<ServiceBusProcessor> GetQueue(string queueName)
		{
			EnsureQueue(queueName);

			// create client
			ServiceBusClient client = new(_configuration.ConnectionString);
			ServiceBusProcessor processor = client.CreateProcessor(
				$"{_configuration.ChannelPrefix}{queueName}",
				new ServiceBusProcessorOptions()
				{
					AutoCompleteMessages = false,
					MaxConcurrentCalls = _configuration.MaxConcurrentCalls,
					MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(4)
				});

			return Task.FromResult(processor);
		}
	}
}
