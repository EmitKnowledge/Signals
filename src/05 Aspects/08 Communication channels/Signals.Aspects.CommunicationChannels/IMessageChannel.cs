using System;
using System.Threading.Tasks;

namespace Signals.Aspects.CommunicationChannels
{
    public interface IMessageChannel
    {
        /// <summary>
        /// Close the subscriber
        /// </summary>
        Task Close();

        /// <summary>
        /// Publish a message
        /// </summary>
        /// <param name="message"></param>
        Task Publish<T>(T message) where T : class;

	    /// <summary>
	    /// Publish a message on a predefined channel
	    /// </summary>
	    /// <param name="channelName"></param>
	    /// <param name="message"></param>
	    Task Publish<T>(string channelName, T message) where T : class;

        /// <summary>
        /// Subscribe to a message type
        /// </summary>
        /// <param name="action"></param>
        Task Subscribe<T>(Action<T> action) where T : class;

        /// <summary>
        /// Subscribe to a message type on a predefined channel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="action"></param>
        Task Subscribe<T>(string channelName, Action<T> action) where T : class;
    }
}
