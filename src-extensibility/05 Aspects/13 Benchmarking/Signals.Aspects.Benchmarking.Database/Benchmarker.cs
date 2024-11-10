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
            CreateBenchmarkTableIfNotExist();
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
		/// <param name="correlationId"></param>
		/// <param name="processName"></param>
		/// <param name="callerProcessName"></param>
		/// <param name="description"></param>
		/// <param name="payload"></param>
		public void Bench(string checkpointName, Guid correlationId, string processName, string callerProcessName = null, string description = null, object payload = null)
        {
            if (!Configuration.IsEnabled) return;

            var entry = new BenchmarkEntry();
            entry.Checkpoint = checkpointName;
            entry.CorrelationId = correlationId;
            entry.ProcessName = processName;
            entry.CallerProcessName = callerProcessName;
            entry.Description = description;
            entry.Payload = payload;
            entry.CreatedOn = DateTime.UtcNow;

            var bag = InMemoryEntiryCache.GetOrAdd(correlationId, new ConcurrentBag<BenchmarkEntry>());
            bag.Add(entry);
        }

		/// <summary>
		/// Persist epic data
		/// </summary>
		/// <param name="correlationId"></param>
		public void Flush(Guid correlationId)
        {
            if (!Configuration.IsEnabled) return;

            bool success = true;

            success = InMemoryNameCache.TryRemove(correlationId, out string banchmarkName);
            if (!success) throw new KeyNotFoundException();

            success = InMemoryEntiryCache.TryRemove(correlationId, out ConcurrentBag<BenchmarkEntry> values);
            if (!success) throw new KeyNotFoundException();

            var entries = values.OrderBy(x => x.CreatedOn);

            DataTable table = new DataTable();

            table.Columns.Add("Id");
            table.Columns.Add("CreatedOn");
            table.Columns.Add("CorrelationId");
            table.Columns.Add("ProcessName");
            table.Columns.Add("CallerProcessName");
            table.Columns.Add("BanchmarkName");
            table.Columns.Add("Checkpoint");
            table.Columns.Add("Description");
            table.Columns.Add("Payload");

            foreach (var entry in entries)
            {
                var row = table.NewRow();
                row["Id"] = 0;
                row["CreatedOn"] = entry.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                row["BanchmarkName"] = banchmarkName;
                row["CorrelationId"] = entry.CorrelationId;
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
        /// <param name="correlationId"></param>
        /// <param name="banchmarkName"></param>
        public void Start(Guid correlationId, string banchmarkName)
        {
            if (!Configuration.IsEnabled) return;

            InMemoryNameCache.AddOrUpdate(correlationId, banchmarkName, (key, value) => value);
        }

        /// <summary>
        /// Get epic report data
        /// </summary>
        /// <param name="banchmarkName"></param>
        /// <param name="afterDate"></param>
        /// <returns></returns>
        public BenchmarkReport GetReport(string banchmarkName, DateTime afterDate)
        {
            if (!Configuration.IsEnabled) return new BenchmarkReport();

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                var sql = $@"SELECT [Id],
                                    [CreatedOn],
                                    [CorrelationId],
                                    [ProcessName],
                                    [CallerProcessName],
                                    [Checkpoint],
                                    [Description],
                                    [Payload] 
                                FROM [{Configuration.TableName}] 
                                WHERE [BanchmarkName] = @BanchmarkName AND [CreatedOn] >= @AfterDate";

                connection.Open();

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("BanchmarkName", banchmarkName);
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
                            CorrelationId = Guid.Parse(reader.GetString(2)),
                            ProcessName = reader.GetString(3),
                            CallerProcessName = callerProcessName.IsNull ? null : callerProcessName.Value,
                            Checkpoint = reader.GetString(5),
                            Description = description.IsNull ? null : description.Value,
                            Payload = payload.IsNull ? null : JsonConvert.DeserializeObject(payload.Value)
                        };

                        result.Add(entry);
                    }
                }

                var reports = result.GroupBy(x => x.CorrelationId).Select(x =>
                {
                    var report = new CorrelationReport(x.Key);
                    report.BenchmarkEntries.AddRange(x.ToList());
                    return report;
                }).ToList();

                var benchmarkReport = new BenchmarkReport();
				benchmarkReport.CorrelationReports.AddRange(reports);

                return benchmarkReport;
            }
        }

        /// <summary>
        /// Ensures that table for the benchmark logs exists in the database
        /// </summary>
        private void CreateBenchmarkTableIfNotExist()
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
                            [CorrelationId] NVARCHAR(MAX) NOT NULL,
                            [ProcessName] NVARCHAR(MAX) NOT NULL,
                            [CallerProcessName] NVARCHAR(MAX) NULL,
                            [BanchmarkName] NVARCHAR(MAX) NULL,
                            [Checkpoint] NVARCHAR(MAX) NOT NULL,
                            [Description] NVARCHAR(MAX) NULL,
                            [Payload] NVARCHAR(MAX) NULL
                            CONSTRAINT [PK_{Configuration.TableName}] PRIMARY KEY CLUSTERED 
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
