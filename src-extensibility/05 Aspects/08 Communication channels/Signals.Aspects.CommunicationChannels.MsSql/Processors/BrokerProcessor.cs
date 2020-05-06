using Signals.Aspects.CommunicationChannels.MsSql.Configurations;
using Signals.Aspects.CommunicationChannels.MsSql.Processors.Base;
using System.Linq;
using System.Threading.Tasks;
using TableDependency.SqlClient;

namespace Signals.Aspects.CommunicationChannels.MsSql.Processors
{
    internal class BrokerProcessor : BaseProcessor
    {
        private SqlTableDependency<SystemMessage> SqlDependency { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public BrokerProcessor(MsSqlMessageChannel channel)
            : base(channel)
        {
        }

        /// <summary>
        /// IDisposable
        /// </summary>
        public override void Dispose()
        {
            SqlDependency.Stop();
        }

        /// <summary>
        /// Init processor
        /// </summary>
        public override void Init()
        {
            // Init only if it hasn't been set before
            if (SqlDependency == null && Channel.Configuration.MessageListeningStrategy == MessageListeningStrategy.Broker)
            {
                SqlDependency = new SqlTableDependency<SystemMessage>(
                    Channel.Configuration.ConnectionString,
                    executeUserPermissionCheck: false,
                    tableName: Channel.Configuration.DbTableName);

                SqlDependency.OnChanged += (o, e) =>
                {
                    if (e.Entity != null && e.Entity.MessageStatus == SystemMessageStatus.Pending /* && Channel.Subscriptions.Any()*/)
                    {
                        var queue = e.Entity?.MessageQueue;

                        if (!string.IsNullOrEmpty(queue) && Channel.Subscriptions.ContainsKey(queue))
                        {
                            var message = Channel.GetAndLockSystemMessageById(e.Entity.Id);
                            Task.Run(() =>
                            {
                                Channel.Subscriptions[queue](message.MessagePayload);
                                Channel.MarkSystemMessageAsProcessed(message.Id);
                            });
                        }
                    }
                };

                // Keep the connection up to 10 mins before destructing the Sql dependency infrastructure from MSSQL
                SqlDependency.Start(timeOut: 60, watchDogTimeOut: 600);
            }
        }
    }
}
