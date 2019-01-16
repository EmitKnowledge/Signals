using Newtonsoft.Json;
using Signals.Aspects.CommunicationChannels.MsSql.Configurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TableDependency.SqlClient;

namespace Signals.Aspects.CommunicationChannels.MsSql
{
    public class MsSqlMessageChannel : IMessageChannel
    {
        private readonly MsSqlChannelConfiguration _configuration;

        /// <summary>
        /// Represents dictionary of subscriptions with queue name and Action for processing the message queue
        /// </summary>
        private Dictionary<string, Action<dynamic>> Subscriptions { get; } = new Dictionary<string, Action<dynamic>>();

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="configuration"></param>
        public MsSqlMessageChannel(MsSqlChannelConfiguration configuration)
        {
            _configuration = configuration;
            CreateDbTableIfDoesntExist();

            if (_configuration.MessageListeningStrategy == MessageListeningStrategy.Broker)
            {
                var dep = new SqlTableDependency<SystemMessage>(_configuration.ConnectionString,
                    executeUserPermissionCheck: false);

                dep.OnChanged += (o, e) =>
                {
                    if (e.Entity != null)
                    {
                        var queue = e.Entity?.MessageQueue;
                        if (Subscriptions.ContainsKey(queue))
                        {
                            Subscriptions[queue](e.Entity.MessagePayload);
                        }
                    }
                };
                dep.Start();
            }
        }

        public Task Close()
        {
            Subscriptions.Clear();
            return Task.CompletedTask;
        }

        public Task Publish<T>(T message) where T : class
        {
            var queueForType = message.GetType();
            var queueName = queueForType.Name;

            return Publish(queueName, message);
        }

        public Task Publish<T>(string channelName, T message) where T : class
        {
            using (var connection = new SqlConnection(_configuration.ConnectionString))
            {
                var serializedMessage = JsonConvert.SerializeObject(message);
                var query =
                    $@"
                        INSERT INTO [{_configuration.DbTableName}](CreatedOn, MessageQueue, MessagePayload)
                        VALUES(@CreatedOn, @MessageQueue, @MessagePayload)
                    ";
                connection.Open();
                var command = new SqlCommand(query, connection);
                command.Parameters.Add("CreatedOn", SqlDbType.DateTime2);
                command.Parameters["CreatedOn"].Value = DateTime.UtcNow;

                command.Parameters.Add("MessageQueue", SqlDbType.NVarChar);
                command.Parameters["MessageQueue"].Value = channelName;

                command.Parameters.Add("MessagePayload", SqlDbType.NVarChar);
                command.Parameters["MessagePayload"].Value = serializedMessage;

                command.ExecuteNonQuery();
            }

            return Task.CompletedTask;
        }

        public Task Subscribe<T>(Action<T> action) where T : class
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var queueForType = typeof(T);
            var queueName = queueForType.Name;

            return Subscribe(queueName, action);
        }

        public Task Subscribe<T>(string channelName, Action<T> action) where T : class
        {
            if (!Subscriptions.ContainsKey(channelName))
            {
                Subscriptions.Add(channelName, action as Action<dynamic>);
            }

            return Task.CompletedTask;
        }

        private void CreateDbTableIfDoesntExist()
        {
            using (var connection = new SqlConnection(_configuration.ConnectionString))
            {
                connection.Open();

                var sql =
                    $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        WHERE t.name = '{_configuration.DbTableName}'
                        ) 
                        CREATE TABLE [{_configuration.DbTableName}]
                        (
                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [CreatedOn] [datetime2](7) NOT NULL,
	                        [MessageQueue] [nvarchar](max) NOT NULL,
	                        [MessagePayload] [nvarchar](max) NOT NULL
                        )
                    ";

                var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
