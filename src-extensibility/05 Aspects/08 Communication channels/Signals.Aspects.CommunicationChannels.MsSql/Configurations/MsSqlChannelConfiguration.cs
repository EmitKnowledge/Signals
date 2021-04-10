using Signals.Aspects.CommunicationChannels.Configurations;

namespace Signals.Aspects.CommunicationChannels.MsSql.Configurations
{
    /// <summary>
    /// Configuration for MsSql
    /// </summary>
    public class MsSqlChannelConfiguration : IChannelConfiguration
    {
        public MsSqlChannelConfiguration()
        {
            DbTableName = "EventMessage";

            if (LongPollingTimeout <= 0) LongPollingTimeout = 100;
        }

        /// <summary>
        /// Database connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Represents the name of the table in the database where the messages are stored
        /// </summary>
        public string DbTableName { get; set; }

        /// <summary>
        /// Represents the listening strategy
        /// </summary>
        public MessageListeningStrategy MessageListeningStrategy { get; set; }

        /// <summary>
        /// Timeout in milliseconds for long polling strategy
        /// </summary>
        public int LongPollingTimeout { get; set; }
    }
}
