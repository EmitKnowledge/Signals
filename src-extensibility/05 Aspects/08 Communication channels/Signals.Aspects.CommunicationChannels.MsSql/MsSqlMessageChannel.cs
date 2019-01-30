﻿using Newtonsoft.Json;
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
        private Dictionary<string, Action<string>> Subscriptions { get; } = new Dictionary<string, Action<string>>();

        private SqlTableDependency<SystemMessage> SqlDependency { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="configuration"></param>
        public MsSqlMessageChannel(MsSqlChannelConfiguration configuration)
        {
            _configuration = configuration;
            CreateDbTableIfDoesntExist();
        }

        public Task Close()
        {
            SqlDependency.Stop();
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
                        INSERT INTO [{_configuration.DbTableName}](CreatedOn, MessageQueue, MessagePayload, MessageStatus)
                        VALUES(@CreatedOn, @MessageQueue, @MessagePayload, @MessageStatus)
                    ";
                connection.Open();
                var command = new SqlCommand(query, connection);

                command.Parameters.Add("CreatedOn", SqlDbType.DateTime2);
                command.Parameters["CreatedOn"].Value = DateTime.UtcNow;

                command.Parameters.Add("MessageQueue", SqlDbType.NVarChar);
                command.Parameters["MessageQueue"].Value = channelName;

                command.Parameters.Add("MessagePayload", SqlDbType.NVarChar);
                command.Parameters["MessagePayload"].Value = serializedMessage;

                command.Parameters.Add("MessageStatus", SqlDbType.Int);
                command.Parameters["MessageStatus"].Value = (int)SystemMessageStatus.Pending;

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
            InitSqlDependency();

            if (!Subscriptions.ContainsKey(channelName))
            {
                void WrappedAction(string messageBody)
                {
                    if (messageBody is T)
                    {
                        action(messageBody as T);
                    }
                    else
                    {
                        var obj = JsonConvert.DeserializeObject<T>(messageBody);
                        action(obj);
                    }
                    
                }

                Subscriptions.Add(channelName, WrappedAction);
            }

            return Task.CompletedTask;
        }

        private void InitSqlDependency()
        {
            // Init only if it hasn't been set before
            if (SqlDependency == null && _configuration.MessageListeningStrategy == MessageListeningStrategy.Broker)
            {
                SqlDependency = new SqlTableDependency<SystemMessage>(_configuration.ConnectionString,
                    executeUserPermissionCheck: false);

                SqlDependency.OnChanged += (o, e) =>
                {
                    if (e.Entity != null)
                    {
                        var message = GetAndLockSystemMessageById(e.Entity.Id);
                        if (message != null)
                        {
                            var queue = message.MessageQueue;
                            if (Subscriptions.ContainsKey(queue))
                            {
                                Subscriptions[queue](message.MessagePayload);
                                MarkSystemMessageAsProcessed(message.Id);
                            }
                        }
                    }
                };
                SqlDependency.Start();
            }
        }

        private void MarkSystemMessageAsProcessed(int messageId)
        {
            using (var connection = new SqlConnection(_configuration.ConnectionString))
            {
                connection.Open();

                var sql =
                    $@"
                        UPDATE [{_configuration.DbTableName}] SET MessageStatus = @ProcessedStatus
                        WHERE Id = @Id
                    ";

                var command = new SqlCommand(sql, connection);

                command.Parameters.Add("Id", SqlDbType.Int);
                command.Parameters["Id"].Value = messageId;

                command.Parameters.Add("ProcessedStatus", SqlDbType.Int);
                command.Parameters["ProcessedStatus"].Value = (int)SystemMessageStatus.Processed;

                command.ExecuteNonQuery();
            }
        }

        private SystemMessage GetAndLockSystemMessageById(int messageId)
        {
            using (var connection = new SqlConnection(_configuration.ConnectionString))
            {
                connection.Open();

                var sql =
                    $@"
                        BEGIN TRANSACTION MSMQMESSAGECHANNEL{messageId}
                            SELECT * FROM [{_configuration.DbTableName}]
                            WHERE Id = @Id AND MessageStatus = @PendingStatus;
                            UPDATE [{_configuration.DbTableName}] SET MessageStatus = @ProcessingStatus
                            WHERE Id = @Id AND MessageStatus = @PendingStatus;
                        COMMIT TRANSACTION MSMQMESSAGECHANNEL{messageId}
                    ";

                var command = new SqlCommand(sql, connection);

                command.Parameters.Add("Id", SqlDbType.Int);
                command.Parameters["Id"].Value = messageId;

                command.Parameters.Add("PendingStatus", SqlDbType.Int);
                command.Parameters["PendingStatus"].Value = (int)SystemMessageStatus.Pending;

                command.Parameters.Add("ProcessingStatus", SqlDbType.Int);
                command.Parameters["ProcessingStatus"].Value = (int)SystemMessageStatus.Processing;


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new SystemMessage
                        {
                            Id = reader.GetInt32(0),
                            CreatedOn = reader.GetDateTime(1),
                            MessageQueue = reader.GetString(2),
                            MessagePayload = reader.GetString(3),
                            MessageStatus = (SystemMessageStatus)reader.GetInt32(4)
                        };
                    }
                }

                return null;
            }
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
	                        [MessagePayload] [nvarchar](max) NOT NULL,
	                        [MessageStatus] [int] NOT NULL
                        )
                    ";

                var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}