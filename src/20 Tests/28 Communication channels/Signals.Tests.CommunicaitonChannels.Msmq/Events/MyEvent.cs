using System;

namespace Signals.Tests.CommunicaitonChannels.Msmq.Events
{
    public class MyEvent
    {
        public string Message { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
