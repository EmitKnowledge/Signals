using Newtonsoft.Json;
using Signals.Aspects.CommunicationChannels.MsSql.Configurations;
using Signals.Aspects.CommunicationChannels.MsSql.Processors;
using Signals.Aspects.CommunicationChannels.MsSql.Processors.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Signals.Aspects.CommunicationChannels.MsSql
{
    public class MsSqlMessageChannel : IMessageChannel
    {
        /// <summary>
        /// MsSql Configuration
        /// </summary>
        internal readonly MsSqlChannelConfiguration Configuration;

        /// <summary>
        /// Represents dictionary of subscriptions with queue name and Action for processing the message queue
        /// </summary>
        internal Dictionary<string, Action<string>> Subscriptions { get; } = new Dictionary<string, Action<string>>();

        /// <summary>
        /// Message processor
        /// </summary>
        private BaseProcessor Processor { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="configuration"></param>
        public MsSqlMessageChannel(MsSqlChannelConfiguration configuration)
        {
            Configuration = configuration;
            CreateDbTableIfDoesntExist();

            if (Configuration.MessageListeningStrategy == MessageListeningStrategy.Broker)
                Processor = new BrokerProcessor(this);
            else if (Configuration.MessageListeningStrategy == MessageListeningStrategy.LongPolling)
                Processor = new LongPollingProcessor(this);

            Processor?.Init();
        }

        /// <summary>
        /// Close message channel
        /// </summary>
        /// <returns></returns>
        public Task Close()
        {
            Processor?.Dispose();
            Subscriptions.Clear();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Publish message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Publish<T>(T message) where T : class
        {
            var queueForType = message.GetType();
            var queueName = queueForType.Name;

            return Publish(queueName, message);
        }

        /// <summary>
        /// Publish message to channel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Publish<T>(string channelName, T message) where T : class
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                var serializedMessage = JsonConvert.SerializeObject(message);
                var query =
                    $@"
                        INSERT INTO [{Configuration.DbTableName}](CreatedOn, MessageQueue, MessagePayload, MessageStatus)
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

        /// <summary>
        /// Subscribe to channel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task Subscribe<T>(Action<T> action) where T : class
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var queueForType = typeof(T);
            var queueName = queueForType.Name;

            return Subscribe(queueName, action);
        }

        /// <summary>
        /// Subscribe to channel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelName"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task Subscribe<T>(string channelName, Action<T> action) where T : class
        {
            // Add the process type to the list of subscribed processes
            if (!Subscriptions.ContainsKey(channelName))
            {
                Subscriptions.Add(channelName, (messageBody) =>
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
                });
            }

            // Execute all unexecuted processes from the queue (database in this case)
            Task.Run(() =>
            {
                ProcessUnprocessedTasks(channelName, action);
            });

            return Task.CompletedTask;
        }

        internal List<SystemMessage> GetAllUnprocessedTasks(string channelName = null)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                var uniqueTransactionId = Guid.NewGuid().ToString().Substring(0, 20).Replace("-", string.Empty);
                var sql =
                    $@"
                        BEGIN TRANSACTION T{uniqueTransactionId}
                            SELECT * FROM [{Configuration.DbTableName}]
                            WHERE MessageStatus = @PendingStatus {(channelName == null ? "" : "AND MessageQueue = @MessageQueue")};
                        COMMIT TRANSACTION T{uniqueTransactionId}
                    ";

                var command = new SqlCommand(sql, connection);

                command.Parameters.Add("PendingStatus", SqlDbType.Int);
                command.Parameters["PendingStatus"].Value = (int)SystemMessageStatus.Pending;

                if (channelName != null)
                {
                    command.Parameters.Add("MessageQueue", SqlDbType.NVarChar);
                    command.Parameters["MessageQueue"].Value = channelName;
                }

                var result = new List<SystemMessage>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new SystemMessage
                        {
                            Id = reader.GetInt32(0),
                            CreatedOn = reader.GetDateTime(1),
                            MessageQueue = reader.GetString(2),
                            MessagePayload = reader.GetString(3),
                            MessageStatus = (SystemMessageStatus)reader.GetInt32(4)
                        });
                    }
                }

                return result;
            }
        }

        internal SystemMessage GetAndLockSystemMessageById(int messageId)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                var uniqueTransactionId = Guid.NewGuid().ToString().Substring(0, 20).Replace("-", string.Empty);
                var sql =
                    $@"
                        BEGIN TRANSACTION T{uniqueTransactionId}
                            SELECT * FROM [{Configuration.DbTableName}]
                            WHERE Id = @Id AND MessageStatus = @PendingStatus;
                            UPDATE [{Configuration.DbTableName}] SET MessageStatus = @ProcessingStatus
                            WHERE Id = @Id AND MessageStatus = @PendingStatus;
                        COMMIT TRANSACTION T{uniqueTransactionId}
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

        internal void MarkSystemMessageAsProcessed(int messageId)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                var sql =
                    $@"
                        UPDATE [{Configuration.DbTableName}] SET MessageStatus = @ProcessedStatus
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

        internal void MarkSystemMessageAsUnprocessed(int messageId)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                var sql =
                    $@"
                        UPDATE [{Configuration.DbTableName}] SET MessageStatus = @PendingStatus
                        WHERE Id = @Id
                    ";

                var command = new SqlCommand(sql, connection);

                command.Parameters.Add("Id", SqlDbType.Int);
                command.Parameters["Id"].Value = messageId;

                command.Parameters.Add("PendingStatus", SqlDbType.Int);
                command.Parameters["PendingStatus"].Value = (int)SystemMessageStatus.Pending;

                command.ExecuteNonQuery();
            }
        }

        private void CreateDbTableIfDoesntExist()
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                var sql =
                    $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        WHERE t.name = '{Configuration.DbTableName}'
                        ) 
                        CREATE TABLE [{Configuration.DbTableName}]
                        (
                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [CreatedOn] [datetime2](7) NOT NULL,
	                        [MessageQueue] [nvarchar](max) NOT NULL,
	                        [MessagePayload] [nvarchar](max) NOT NULL,
	                        [MessageStatus] [int] NOT NULL
                            CONSTRAINT [PK_{Configuration.DbTableName}] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                    ";

                var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();

                try
                {
                    var indexSql = $@"";

                    var indexCommand = new SqlCommand(indexSql, connection);
                    indexCommand.ExecuteNonQuery();
                }
                catch { }
            }
        }

        private void ProcessUnprocessedTasks<T>(string channelName, Action<T> action) where T : class
        {
            // Get all unprocessed tasks in the channel and process them all
            var unprocessedTasks = GetAllUnprocessedTasks(channelName);
            foreach (var unprocessedTask in unprocessedTasks)
            {
                try
                {
                    if (unprocessedTask.MessagePayload is T)
                    {
                        action(unprocessedTask.MessagePayload as T);
                    }
                    else
                    {
                        var obj = JsonConvert.DeserializeObject<T>(unprocessedTask.MessagePayload);
                        action(obj);
                    }

                    MarkSystemMessageAsProcessed(unprocessedTask.Id);
                }
                catch
                {
                    MarkSystemMessageAsUnprocessed(unprocessedTask.Id);
                }
            }
        }
    }
}
