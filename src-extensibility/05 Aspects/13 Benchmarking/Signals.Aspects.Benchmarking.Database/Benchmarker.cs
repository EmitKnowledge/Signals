using Newtonsoft.Json;
using Signals.Aspects.Benchmarking.Database.Configurations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Signals.Aspects.Benchmarking.Database
{
    /// <summary>
    /// Benchmark engine
    /// </summary>
    public class Benchmarker : IBenchmarker
    {
        /// <summary>
        /// Static CTOR
        /// </summary>
        static Benchmarker()
        {
            InMemoryNameCache = new ConcurrentDictionary<Guid, string>();
            InMemoryEntiryCache = new ConcurrentDictionary<Guid, ConcurrentBag<BenchmarkEntry>>();
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="databaseConfiguration"></param>
        public Benchmarker(DatabaseBenchmarkingConfiguration databaseConfiguration)
        {
            Configuration = databaseConfiguration;
            CreateAuditTableIfNotExist();
        }

        /// <summary>
        /// Benchmarker configuration
        /// </summary>
        public DatabaseBenchmarkingConfiguration Configuration { get; set; }

        private static ConcurrentDictionary<Guid, ConcurrentBag<BenchmarkEntry>> InMemoryEntiryCache { get; set; }
        private static ConcurrentDictionary<Guid, string> InMemoryNameCache { get; set; }

        /// <summary>
        /// Hit benchmarking checkpoint
        /// </summary>
        /// <param name="checkpointName"></param>
        /// <param name="epicId"></param>
        /// <param name="processName"></param>
        /// <param name="callerProcessName"></param>
        /// <param name="description"></param>
        /// <param name="payload"></param>
        public void Bench(string checkpointName, Guid epicId, string processName, string callerProcessName = null, string description = null, object payload = null)
        {
            if (!Configuration.IsEnabled) return;

            var entry = new BenchmarkEntry();
            entry.Checkpoint = checkpointName;
            entry.EpicId = epicId;
            entry.ProcessName = processName;
            entry.CallerProcessName = callerProcessName;
            entry.Description = description;
            entry.Payload = payload;
            entry.CreatedOn = DateTime.UtcNow;

            var bag = InMemoryEntiryCache.GetOrAdd(epicId, new ConcurrentBag<BenchmarkEntry>());
            bag.Add(entry);
        }

        /// <summary>
        /// Persist epic data
        /// </summary>
        /// <param name="epicId"></param>
        public void FlushEpic(Guid epicId)
        {
            if (!Configuration.IsEnabled) return;

            bool success = true;

            success = InMemoryNameCache.TryRemove(epicId, out string epicName);
            if (!success) throw new KeyNotFoundException();

            success = InMemoryEntiryCache.TryRemove(epicId, out ConcurrentBag<BenchmarkEntry> values);
            if (!success) throw new KeyNotFoundException();

            var entries = values.OrderBy(x => x.CreatedOn);

            DataTable table = new DataTable();

            table.Columns.Add("Id");
            table.Columns.Add("CreatedOn");
            table.Columns.Add("EpicId");
            table.Columns.Add("ProcessName");
            table.Columns.Add("CallerProcessName");
            table.Columns.Add("EpicName");
            table.Columns.Add("Checkpoint");
            table.Columns.Add("Description");
            table.Columns.Add("Payload");

            foreach (var entry in entries)
            {
                var row = table.NewRow();
                row["Id"] = 0;
                row["CreatedOn"] = entry.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                row["EpicName"] = epicName;
                row["EpicId"] = entry.EpicId;
                row["ProcessName"] = entry.ProcessName;
                row["Checkpoint"] = entry.Checkpoint;
                row["CallerProcessName"] = string.IsNullOrEmpty(entry.CallerProcessName) ? DBNull.Value : (object)entry.CallerProcessName;
                row["Description"] = string.IsNullOrEmpty(entry.Description) ? DBNull.Value : (object)entry.Description;
                row["Payload"] = entry.Payload == null ? DBNull.Value : (object)JsonConvert.SerializeObject(entry.Payload);

                table.Rows.Add(row);
            }

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BatchSize = 500;
                    bulkCopy.DestinationTableName = Configuration.TableName;
                    try
                    {
                        bulkCopy.WriteToServer(table);
                        transaction.Commit();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Mark epic as started
        /// </summary>
        /// <param name="epicId"></param>
        /// <param name="epicName"></param>
        public void StartEpic(Guid epicId, string epicName)
        {
            if (!Configuration.IsEnabled) return;

            InMemoryNameCache.AddOrUpdate(epicId, epicName, (key, value) => value);
        }

        /// <summary>
        /// Get epic report data
        /// </summary>
        /// <param name="epicName"></param>
        /// <param name="afterDate"></param>
        /// <returns></returns>
        public EpicsReport GetEpicReport(string epicName, DateTime afterDate)
        {
            if (!Configuration.IsEnabled) return new EpicsReport();

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                var sql = $@"SELECT [Id],
                                    [CreatedOn],
                                    [EpicId],
                                    [ProcessName],
                                    [CallerProcessName],
                                    [Checkpoint],
                                    [Description],
                                    [Payload] 
                                FROM [{Configuration.TableName}] 
                                WHERE [EpicName] = @EpicName AND [CreatedOn] >= @AfterDate";

                connection.Open();

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("EpicName", epicName);
                command.Parameters.AddWithValue("AfterDate", afterDate.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));

                var result = new List<BenchmarkEntry>();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var callerProcessName = reader.GetSqlString(4);
                        var description = reader.GetSqlString(6);
                        var payload = reader.GetSqlString(7);

                        var entry = new BenchmarkEntry
                        {
                            Id = reader.GetInt32(0),
                            CreatedOn = reader.GetDateTime(1),
                            EpicId = Guid.Parse(reader.GetString(2)),
                            ProcessName = reader.GetString(3),
                            CallerProcessName = callerProcessName.IsNull ? null : callerProcessName.Value,
                            Checkpoint = reader.GetString(5),
                            Description = description.IsNull ? null : description.Value,
                            Payload = payload.IsNull ? null : JsonConvert.DeserializeObject(payload.Value)
                        };

                        result.Add(entry);
                    }
                }

                var reports = result.GroupBy(x => x.EpicId).Select(x =>
                {
                    var report = new EpicReport(x.Key);

                    report.BenchmarkEntries.AddRange(x.ToList());

                    return report;
                }).ToList();

                var epicReport = new EpicsReport();
                epicReport.EpicReports.AddRange(reports);

                return epicReport;
            }
        }

        /// <summary>
        /// Ensures that table for the audit logs exists in the database
        /// </summary>
        private void CreateAuditTableIfNotExist()
        {
            if (!Configuration.IsEnabled) return;

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                var sql = $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        WHERE t.name = '{Configuration.TableName}'
                        ) 
                        CREATE TABLE [{Configuration.TableName}]
                        (
                            [Id] INT IDENTITY(1,1) NOT NULL, 
                            [CreatedOn] datetime2(7) NOT NULL DEFAULT getutcdate(),
                            [EpicId] NVARCHAR(MAX) NOT NULL,
                            [ProcessName] NVARCHAR(MAX) NOT NULL,
                            [CallerProcessName] NVARCHAR(MAX) NULL,
                            [EpicName] NVARCHAR(MAX) NULL,
                            [Checkpoint] NVARCHAR(MAX) NOT NULL,
                            [Description] NVARCHAR(MAX) NULL,
                            [Payload] NVARCHAR(MAX) NULL
                        )
                    ";

                var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
