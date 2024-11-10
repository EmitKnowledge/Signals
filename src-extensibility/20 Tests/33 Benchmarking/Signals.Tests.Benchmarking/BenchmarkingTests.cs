using Signals.Aspects.Benchmarking;
using Signals.Aspects.Benchmarking.Database;
using Signals.Aspects.Benchmarking.Database.Configurations;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Signals.Tests.Configuration;
using Xunit;

namespace Signals.Tests.Benchmarking
{
    public class BenchmarkingTests
    {
        private readonly DatabaseBenchmarkingConfiguration _databaseConfiguration;
        private readonly IBenchmarker _benchmarker;
        
        private static BaseTestConfiguration _configuration  = BaseTestConfiguration.Instance;

        public BenchmarkingTests()
        {
            _databaseConfiguration = new DatabaseBenchmarkingConfiguration
            {
                ConnectionString = _configuration.DatabaseConfiguration.ConnectionString,
                TableName = "BenchmarkEntry2"
            };

            // Clear the database
            ClearDb();

            _benchmarker = new Benchmarker(_databaseConfiguration);
        }

        [Fact]
        public void Benchmarking_PersistsInDatabase_GeneratesReport()
        {
            Guid correlationId = Guid.NewGuid();
            string banchmarkName = "My epic";
            string processName = "MyProc";

            _benchmarker.Start(correlationId, banchmarkName);
            _benchmarker.Bench("Start", correlationId, processName);
            Thread.Sleep(100);
            _benchmarker.Bench("Processing", correlationId, processName);
            Thread.Sleep(100);
            _benchmarker.Bench("End", correlationId, processName);
            _benchmarker.Flush(correlationId);
            var report = _benchmarker.GetReport(banchmarkName, DateTime.UtcNow.AddMinutes(-1));

            Assert.Single(report.CorrelationReports);
            Assert.Contains(report.CorrelationReports, x => x.CorrelationId == correlationId);
            Assert.Equal(3, report.CorrelationReports.SingleOrDefault(x => x.CorrelationId == correlationId)?.BenchmarkEntries.Count);

            Assert.Equal("Start", report.CorrelationReports.SingleOrDefault(x => x.CorrelationId == correlationId).BenchmarkEntries[0].Checkpoint);
            Assert.Equal("Processing", report.CorrelationReports.SingleOrDefault(x => x.CorrelationId == correlationId).BenchmarkEntries[1].Checkpoint);
            Assert.Equal("End", report.CorrelationReports.SingleOrDefault(x => x.CorrelationId == correlationId).BenchmarkEntries[2].Checkpoint);

        }

        private void ClearDb()
        {
            try
            {
                using (var conn = new SqlConnection(_databaseConfiguration.ConnectionString))
                {
                    var sql = $"DROP TABLE [{_databaseConfiguration.TableName}]";
                    conn.Open();
                    new SqlCommand(sql, conn).ExecuteNonQuery();
                }
            }
            catch
            { }
        }
    }
}
