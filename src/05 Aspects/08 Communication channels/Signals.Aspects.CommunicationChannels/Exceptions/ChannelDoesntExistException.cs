using System;

namespace Signals.Aspects.CommunicationChannels.Exceptions
{
    public class ChannelDoesntExistException : Exception
    {
        public ChannelDoesntExistException(string channelName): base($"Channel {channelName} does not exist.")
        {
        }
    }
}
