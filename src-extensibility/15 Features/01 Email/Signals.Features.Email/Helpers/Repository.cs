using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Signals.Features.Email.Helpers
{
    /// <summary>
    /// Database repository
    /// </summary>
    internal class Repository
    {
        private readonly string connectionString;
        private readonly string tableName;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        public Repository(string connectionString, string tableName)
        {
            this.connectionString = connectionString;
            this.tableName = tableName;

            CreateTableIfDoesntExist();
        }

        /// <summary>
        /// Get due emails that should be sent
        /// </summary>
        /// <returns></returns>
        public List<EmailMessage> GetDueEmails()
        {
            var emailMessages = new List<EmailMessage>();

            using (var connection = GetConnection())
            {
                var sql =
                    $@"
                        SELECT * FROM [{tableName}] WHERE [ScheduledFor] <= GETUTCDATE() AND [IsSent] = 0 AND [Error] IS NULL
                    ";
                connection.Open();
                var command = new SqlCommand(sql, connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        emailMessages.Add(new EmailMessage
                        {
                            Id = (int)reader["Id"],
                            CreatedOn = (DateTime)reader["CreatedOn"],
                            Key = reader["Key"].ToString(),
                            ScheduledFor = (DateTime?)reader["ScheduledFor"],
                            IsSent = (bool)reader["IsSent"],
                            Sender = reader["Sender"].ToString(),
                            From = reader["From"].ToString(),
                            Subject = reader["Subject"].ToString(),
                            Body = reader["Body"].ToString(),
                            To = SafeDeserialize<List<string>>(reader["To"].ToString()),
                            Cc = SafeDeserialize<List<string>>(reader["Cc"].ToString()),
                            Bcc = SafeDeserialize<List<string>>(reader["Bcc"].ToString()),
                            IsBodyHtml = (bool)reader["IsBodyHtml"],
                            SubjectEncoding = reader["SubjectEncoding"].ToString(),
                            BodyEncoding = reader["BodyEncoding"].ToString(),
                            ReplyTo = SafeDeserialize<List<string>>(reader["ReplyTo"].ToString()),
                            Attachments = SafeDeserialize<List<EmailAttachment>>(reader["Attachments"].ToString()),
                            Error = reader["Error"].ToString(),
                        });
                    }
                }
            }

            return emailMessages;
        }

        /// <summary>
        /// Mark email as sent
        /// </summary>
        /// <param name="id"></param>
        public void MarkAsSent(int id)
        {
            using (var connection = GetConnection())
            {
                var sql =
                    $@"
                        UPDATE [{tableName}] SET [IsSent] = 1 WHERE [Id] = @Id
                    ";
                connection.Open();
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Id", id);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Get failed emails between dates
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<EmailMessage> GetFailedBetweenDates(DateTime start, DateTime end)
        {
            var emailMessages = new List<EmailMessage>();

            using (var connection = GetConnection())
            {
                var sql =
                    $@"
                        SELECT * FROM [{tableName}] WHERE [CreatedOn] >= @Start AND [CreatedOn] <= @End AND [IsSent] = 0 AND [Error] IS NOT NULL
                    ";
                connection.Open();
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Start", start);
                command.Parameters.AddWithValue("End", end);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        emailMessages.Add(new EmailMessage
                        {
                            Id = (int)reader["Id"],
                            CreatedOn = (DateTime)reader["CreatedOn"],
                            Key = reader["Key"].ToString(),
                            ScheduledFor = reader["ScheduledFor"] == DBNull.Value ? default(DateTime?) : (DateTime?)reader["ScheduledFor"],
                            IsSent = (bool)reader["IsSent"],
                            Sender = reader["Sender"].ToString(),
                            From = reader["From"].ToString(),
                            Subject = reader["Subject"].ToString(),
                            Body = reader["Body"].ToString(),
                            To = SafeDeserialize<List<string>>(reader["To"].ToString()),
                            Cc = SafeDeserialize<List<string>>(reader["Cc"].ToString()),
                            Bcc = SafeDeserialize<List<string>>(reader["Bcc"].ToString()),
                            IsBodyHtml = (bool)reader["IsBodyHtml"],
                            SubjectEncoding = reader["SubjectEncoding"].ToString(),
                            BodyEncoding = reader["BodyEncoding"].ToString(),
                            ReplyTo = SafeDeserialize<List<string>>(reader["ReplyTo"].ToString()),
                            Attachments = SafeDeserialize<List<EmailAttachment>>(reader["Attachments"].ToString()),
                            Error = reader["Error"].ToString(),
                        });
                    }
                }
            }

            return emailMessages;
        }

        /// <summary>
        /// Set email sending error
        /// </summary>
        /// <param name="id"></param>
        /// <param name="error"></param>
        public void MarkAsError(int id, string error)
        {
            using (var connection = GetConnection())
            {
                var sql =
                    $@"
                        UPDATE [{tableName}] SET [Error] = @Error WHERE [Id] = @Id
                    ";
                connection.Open();
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Error", error);
                command.Parameters.AddWithValue("Id", id);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailMessage"></param>
        public void Insert(EmailMessage emailMessage)
        {
            using (var connection = GetConnection())
            {
                var sql =
                    $@"
                        INSERT INTO [{tableName}] ([Key], [ScheduledFor], [IsSent], [Sender], [From], [Subject], [Body], [To], [Cc], [Bcc], [IsBodyHtml], [SubjectEncoding], [BodyEncoding], [ReplyTo], [Attachments], [Error]) 
                        VALUES (@Key, @ScheduledFor, @IsSent, @Sender, @From, @Subject, @Body, @To, @Cc, @Bcc, @IsBodyHtml, @SubjectEncoding, @BodyEncoding, @ReplyTo, @Attachments, @Error)
                    ";

                connection.Open();
                var command = new SqlCommand(sql, connection);

                if (string.IsNullOrEmpty(emailMessage.Key))
                    command.Parameters.AddWithValue("Key", DBNull.Value);
                else
                    command.Parameters.AddWithValue("Key", emailMessage.Key);

                if (!emailMessage.ScheduledFor.HasValue)
                    command.Parameters.AddWithValue("ScheduledFor", DBNull.Value);
                else
                    command.Parameters.AddWithValue("ScheduledFor", emailMessage.ScheduledFor);

                command.Parameters.AddWithValue("IsSent", emailMessage.IsSent ? 1 : 0);

                if (string.IsNullOrEmpty(emailMessage.Sender))
                    command.Parameters.AddWithValue("Sender", DBNull.Value);
                else
                    command.Parameters.AddWithValue("Sender", emailMessage.Sender);

                command.Parameters.AddWithValue("From", emailMessage.From);
                command.Parameters.AddWithValue("Subject", emailMessage.Subject);
                command.Parameters.AddWithValue("Body", emailMessage.Body);

                if (emailMessage.To?.Any() != true)
                    command.Parameters.AddWithValue("To", DBNull.Value);
                else
                    command.Parameters.AddWithValue("To", SafeSerialize(emailMessage.To));

                if (emailMessage.Cc?.Any() != true)
                    command.Parameters.AddWithValue("Cc", DBNull.Value);
                else
                    command.Parameters.AddWithValue("Cc", SafeSerialize(emailMessage.Cc));

                if (emailMessage.Bcc?.Any() != true)
                    command.Parameters.AddWithValue("Bcc", DBNull.Value);
                else
                    command.Parameters.AddWithValue("Bcc", SafeSerialize(emailMessage.Bcc));

                command.Parameters.AddWithValue("IsBodyHtml", emailMessage.IsBodyHtml ? 1 : 0);

                if (string.IsNullOrEmpty(emailMessage.SubjectEncoding))
                    command.Parameters.AddWithValue("SubjectEncoding", DBNull.Value);
                else
                    command.Parameters.AddWithValue("SubjectEncoding", emailMessage.SubjectEncoding);

                if (string.IsNullOrEmpty(emailMessage.BodyEncoding))
                    command.Parameters.AddWithValue("BodyEncoding", DBNull.Value);
                else
                    command.Parameters.AddWithValue("BodyEncoding", emailMessage.BodyEncoding);

                if (emailMessage.ReplyTo?.Any() != true)
                    command.Parameters.AddWithValue("ReplyTo", DBNull.Value);
                else
                    command.Parameters.AddWithValue("ReplyTo", SafeSerialize(emailMessage.ReplyTo));

                if (emailMessage.Attachments?.Any() != true)
                    command.Parameters.AddWithValue("Attachments", DBNull.Value);
                else
                    command.Parameters.AddWithValue("Attachments", SafeSerialize(emailMessage.Attachments));

                if (string.IsNullOrEmpty(emailMessage.Error))
                    command.Parameters.AddWithValue("Error", DBNull.Value);
                else
                    command.Parameters.AddWithValue("Error", emailMessage.Error);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Unschedule email
        /// </summary>
        /// <param name="key"></param>
        public void Unschedule(string key)
        {
            using (var connection = GetConnection())
            {
                var sql =
                    $@"
                        UPDATE [{tableName}] SET [ScheduledFor] = NULL WHERE [Key] = @Key
                    ";
                connection.Open();
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Key", key);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Create table if doesn't exist
        /// </summary>
        private void CreateTableIfDoesntExist()
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                var sql =
                    $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        WHERE t.name = '{tableName}'
                        ) 
                        CREATE TABLE [{tableName}]
                        (
                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [CreatedOn] datetime2(7) NOT NULL DEFAULT CURRENT_TIMESTAMP,
	                        [Key] nvarchar(128) NULL,
	                        [ScheduledFor] datetime2(7) NULL,
	                        [IsSent] bit NOT NULL,
	                        [Sender] nvarchar(320) NULL,
	                        [From] nvarchar(320) NOT NULL,
	                        [Subject] nvarchar(MAX) NOT NULL,
	                        [Body] nvarchar(MAX) NOT NULL,
	                        [To] nvarchar(MAX) NOT NULL,
	                        [Cc] nvarchar(MAX) NULL,
	                        [Bcc] nvarchar(MAX) NULL,
	                        [IsBodyHtml] bit NOT NULL,
	                        [SubjectEncoding] nvarchar(16) NULL,
	                        [BodyEncoding] nvarchar(16) NULL,
	                        [ReplyTo] nvarchar(MAX) NULL,
	                        [Attachments] nvarchar(MAX) NULL,
	                        [Error] nvarchar(MAX) NULL,
                            CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                    ";

                var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Get database connection
        /// </summary>
        /// <returns></returns>
        private SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        private T SafeDeserialize<T>(string str) where T : class, new()
        {
            if (string.IsNullOrEmpty(str)) return null;

            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string SafeSerialize(object obj)
        {
            if (obj == null) return null;
            return JsonConvert.SerializeObject(obj);
        }
    }
}
