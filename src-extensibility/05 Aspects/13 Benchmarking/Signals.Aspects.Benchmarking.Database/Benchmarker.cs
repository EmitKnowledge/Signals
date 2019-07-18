using Newtonsoft.Json;
using Signals.Aspects.Benchmarking.Database.Configurations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
            CreateAuditTableIfNotExist(databaseConfiguration);
            Configuration = databaseConfiguration;
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
        /// <param name="description"></param>
        /// <param name="payload"></param>
        public void Bench(string checkpointName, Guid epicId, string description = null, object payload = null)
        {
            var entry = new BenchmarkEntry();
            entry.Checkpoint = checkpointName;
            entry.EpicId = epicId;
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
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                bool success = true;

                success = InMemoryNameCache.TryRemove(epicId, out string epicName);
                if (!success) throw new KeyNotFoundException();

                success = InMemoryEntiryCache.TryRemove(epicId, out ConcurrentBag<BenchmarkEntry> values);
                if (!success) throw new KeyNotFoundException();

                var entries = values.OrderBy(x => x.CreatedOn);

                connection.Open();
                foreach (var entry in entries)
                {
                    var hasDescription = !string.IsNullOrEmpty(entry.Description);
                    var hasPayload = entry.Payload != null;

                    var sql = $@"INSERT INTO [{Configuration.TableName}] ([EpicName], [EpicId], [Checkpoint]{(hasDescription ? ", [Description]" : "")}{(hasPayload ? ", [Payload]" : "")}) 
                             VALUES (@EpicName, @EpicId, @Checkpoint{(hasDescription ? ", @Description" : "")}{(hasPayload ? ", @Payload" : "")})";

                    var command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("EpicName", epicName);
                    command.Parameters.AddWithValue("EpicId", entry.EpicId.ToString());
                    command.Parameters.AddWithValue("Checkpoint", entry.Checkpoint);

                    if (hasDescription) command.Parameters.AddWithValue("Description", entry.Description);
                    if (hasPayload) command.Parameters.AddWithValue("Payload", JsonConvert.SerializeObject(entry.Payload));

                    command.ExecuteNonQuery();
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
            InMemoryNameCache.AddOrUpdate(epicId, epicName, (key, value) => throw new ArgumentException("An item with the same key has already been added."));
        }

        /// <summary>
        /// Get epic report data
        /// </summary>
        /// <param name="epicName"></param>
        public Dictionary<Guid, List<BenchmarkEntry>> GetEpicReport(string epicName)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                var sql = $@"SELECT [Id],
                                    [CreatedOn],
                                    [EpicId],
                                    [Checkpoint],
                                    [Description],
                                    [Payload] 
                                FROM [{Configuration.TableName}] 
                                WHERE EpicName = @EpicName";

                connection.Open();

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("EpicName", epicName);

                var result = new List<BenchmarkEntry>();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var description = reader.GetSqlString(4);
                        var payload = reader.GetSqlString(5);
                        var entry = new BenchmarkEntry
                        {
                            Id = reader.GetInt32(0),
                            CreatedOn = reader.GetDateTime(1),
                            EpicId = Guid.Parse(reader.GetString(2)),
                            Checkpoint = reader.GetString(3),
                            Description = description.IsNull ? null : description.Value,
                            Payload = payload.IsNull ? null : JsonConvert.DeserializeObject(payload.Value)
                        };

                        result.Add(entry);
                    }
                }

                return result.GroupBy(x => x.EpicId).ToDictionary(x => x.Key, x => x.ToList());
            }
        }

        /// <summary>
        /// Ensures that table for the audit logs exists in the database
        /// </summary>
        /// <param name="databaseConfiguration"></param>
        private void CreateAuditTableIfNotExist(DatabaseBenchmarkingConfiguration databaseConfiguration)
        {
            using (var connection = new SqlConnection(databaseConfiguration.ConnectionString))
            {
                connection.Open();

                var sql = $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        WHERE t.name = '{databaseConfiguration.TableName}'
                        ) 
                        CREATE TABLE [{databaseConfiguration.TableName}]
                        (
                            [Id] INT IDENTITY(1,1) NOT NULL, 
                            [CreatedOn] datetime2(7) NOT NULL DEFAULT getutcdate(),
                            [EpicId] NVARCHAR(MAX) NOT NULL,
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
