using System;
using System.Data;
using System.Data.SqlClient;
using Audit.Core;
using Newtonsoft.Json;
using Signals.Aspects.Auditing.AuditNET.Configurations;

namespace Signals.Aspects.Auditing.AuditNET.DataProviders
{
    public class DatabaseDataProvider : AuditDataProvider
    {
        private readonly DatabaseAuditingConfiguration _databaseConfiguration;

		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="databaseConfiguration"></param>
        public DatabaseDataProvider(DatabaseAuditingConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration;
        }

        /// <summary>
        /// Inserts an audit event into MSSQL database
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <returns></returns>
        public override object InsertEvent(AuditEvent auditEvent)
        {
            using (var connection = new SqlConnection(_databaseConfiguration.ConnectionString))
            {
                connection.Open();

                var customAuditEvent = (SignalsAuditEvent)auditEvent;

                var sql =
                    $@"
                        INSERT INTO dbo.[{_databaseConfiguration.TableName}]
                        (Process, ProcessInstanceId, EventType, StartDate, EndDate, Originator, [Data])
                        VALUES
                        (
                            @Process, 
                            @ProcessInstanceId, 
                            @EventType, 
                            @StartDate, 
                            @EndDate,
                            @Originator,
                            @Data
                        )
                    ";

                var command = new SqlCommand(sql, connection);

	            // set the process value
	            command.Parameters.Add("Process", SqlDbType.NVarChar);
	            if (customAuditEvent.Process != null)
	            {
		            command.Parameters["Process"].Value = customAuditEvent.Process;
	            }
	            else
	            {
					throw new ArgumentException("Value must be provided", nameof(customAuditEvent.Process));
	            }

				// set the process instance
				command.Parameters.Add("ProcessInstanceId", SqlDbType.NVarChar);
	            if (customAuditEvent.ProcessInstanceId != null)
	            {
		            command.Parameters["ProcessInstanceId"].Value = customAuditEvent.ProcessInstanceId;
	            }
	            else
	            {
					throw new ArgumentException("Value must be provided", nameof(customAuditEvent.ProcessInstanceId));
				}

				// set the process event type
				command.Parameters.Add("EventType", SqlDbType.NVarChar);
	            if (customAuditEvent.EventType != null)
	            {
		            command.Parameters["EventType"].Value = customAuditEvent.EventType;
	            }
	            else
	            {
					throw new ArgumentException("Value must be provided", nameof(customAuditEvent.EventType));
				}

				// set the process start date
				command.Parameters.Add("StartDate", SqlDbType.DateTime2);
	            if (customAuditEvent.StartDate >= DateTime.MinValue)
	            {
		            command.Parameters["StartDate"].Value = customAuditEvent.StartDate;
	            }
	            else
	            {
					throw new ArgumentException("Valid value must be provided", nameof(customAuditEvent.StartDate));
				}

				// set the process end date
				command.Parameters.Add("EndDate", SqlDbType.DateTime2);
	            if (customAuditEvent.EndDate.HasValue)
	            {
		            command.Parameters["EndDate"].Value = customAuditEvent.EndDate;
	            }
	            else
	            {
		            command.Parameters["EndDate"].Value = System.DBNull.Value;
	            }

				// set the process originator
				command.Parameters.Add("Originator", SqlDbType.NVarChar);
	            if (!string.IsNullOrEmpty(customAuditEvent.Originator))
	            {
		            command.Parameters["Originator"].Value = customAuditEvent.Originator;
	            }
	            else
	            {
					throw new ArgumentException("Value must be provided", nameof(customAuditEvent.Originator));
				}

				// set the process instance payload
				command.Parameters.Add("Data", SqlDbType.NVarChar);
	            if (customAuditEvent.Data != null)
	            {
					command.Parameters["Data"].Value = JsonConvert.SerializeObject(customAuditEvent.Data);
				}
				else
	            {
		            command.Parameters["Data"].Value = System.DBNull.Value;
				}

				command.ExecuteNonQuery();

                return auditEvent;
            }
        }
    }
}
