﻿using Audit.Core;
using Audit.Core.Providers;
using Signals.Aspects.Auditing.AuditNET.Configurations;
using Signals.Aspects.Auditing.AuditNET.DataProviders;
using System;
using System.Data.SqlClient;

namespace Signals.Aspects.Auditing.AuditNET
{
    /// <summary>
    /// Auditing provider
    /// </summary>
    public class AuditProvider : IAuditProvider
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="fileConfiguration"></param>
        public AuditProvider(FileAuditingConfiguration fileConfiguration)
        {
            var fileDataProvider = new FileDataProvider
            {
                DirectoryPath = fileConfiguration.DirectoryPath,
                FilenameBuilder = x => fileConfiguration.FilenameBuilder?.Invoke()
            };

            Configuration.DataProvider = fileDataProvider;
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="databaseConfiguration"></param>
        public AuditProvider(DatabaseAuditingConfiguration databaseConfiguration)
        {
            var databaseDataProvider = new DatabaseDataProvider(databaseConfiguration);
            CreateAuditTableIfNotExist(databaseConfiguration);
            Configuration.DataProvider = databaseDataProvider;
        }

        /// <summary>
        /// Creates audit for the audit entry
        /// </summary>
        /// <param name="auditEntry"></param>
        /// <param name="action"></param>
        public void Audit(AuditEntry auditEntry, Action action)
        {
            using (var audit = AuditScope.Create(new AuditScopeOptions { AuditEvent = (SignalsAuditEvent)auditEntry, EventType = auditEntry.EventType }))
            {
                action();
                audit.Event.Environment = new AuditEventEnvironment();
            }
        }

        /// <summary>
        /// Creates audit for the audit entry
        /// </summary>
        /// <param name="auditEntry"></param>
        /// <param name="action"></param>
        public T Audit<T>(AuditEntry auditEntry, Func<T> action)
        {
            using (var audit = AuditScope.Create(new AuditScopeOptions { AuditEvent = (SignalsAuditEvent)auditEntry, EventType = auditEntry.EventType }))
            {
                var result = action();
                audit.Event.Environment = new AuditEventEnvironment();
                return result;
            }
        }

        /// <summary>
        /// Creates new audit entry and returns it
        /// </summary>
        /// <returns></returns>
        public AuditEntry Entry()
        {
            return new AuditEntry(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Ensures that table for the audit logs exists in the database
        /// </summary>
        /// <param name="databaseConfiguration"></param>
        private void CreateAuditTableIfNotExist(DatabaseAuditingConfiguration databaseConfiguration)
        {
            using (var connection = new SqlConnection(databaseConfiguration.ConnectionString))
            {
                connection.Open();

                var sql =
                    $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        WHERE t.name = '{databaseConfiguration.TableName}'
                        ) 
                        CREATE TABLE [{databaseConfiguration.TableName}]
                        (
                            [Id] INT IDENTITY(1,1) NOT NULL, 
                            [Process] NVARCHAR(MAX) NOT NULL,
                            [ProcessInstanceId] NVARCHAR(MAX) NOT NULL,
                            [EventType] NVARCHAR(MAX) NULL,
                            [Originator] NVARCHAR(MAX) NOT NULL,
                            [StartDate] datetime2(7) NOT NULL,
                            [EndDate] datetime2(7) NULL,
                            [Data] NVARCHAR(MAX) NULL,
                            [Payload] NVARCHAR(MAX) NULL,
                            [CorrelationId] NVARCHAR(38) NOT NULL
                            CONSTRAINT [PK_{databaseConfiguration.TableName}] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                    ";

                var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
