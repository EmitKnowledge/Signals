using System;

namespace Signals.Aspects.CommunicationChannels.Exceptions
{
    /// <summary>
    /// Channel exception
    /// </summary>
    public class ChannelDoesntExistException : Exception
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="channelName"></param>
        public ChannelDoesntExistException(string channelName): base($"Channel {channelName} does not exist.")
        {
        }
    }
}
