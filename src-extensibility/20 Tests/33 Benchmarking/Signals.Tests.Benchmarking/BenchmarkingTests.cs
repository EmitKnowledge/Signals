using Signals.Aspects.Benchmarking;
using Signals.Aspects.Benchmarking.Database;
using Signals.Aspects.Benchmarking.Database.Configurations;
using System;
using System.Data.SqlClient;
using System.Threading;
using Xunit;

namespace Signals.Tests.Benchmarking
{
    public class BenchmarkingTests
    {
        private readonly DatabaseBenchmarkingConfiguration _databaseConfiguration;
        private readonly IBenchmarker _benchmarker;

        public BenchmarkingTests()
        {
            _databaseConfiguration = new DatabaseBenchmarkingConfiguration
            {
                ConnectionString = "Server=sql.emitknowledge.com;Database=app.db;User Id=appusr;Password=FYGncRXGySXDz6RFNg2e;"
            };

            // Clear the database
            ClearDb();

            _benchmarker = new Benchmarker(_databaseConfiguration);
        }

        [Fact]
        public void Benchmarking_PersistsInDatabase_GeneratesReport()
        {
            Guid epicPass1 = Guid.NewGuid();
            string epicName = "My epic";
            string processName = "MyProc";

            _benchmarker.StartEpic(epicPass1, epicName);
            _benchmarker.Bench("Start", epicPass1, processName);
            Thread.Sleep(100);
            _benchmarker.Bench("Processing", epicPass1, processName);
            Thread.Sleep(100);
            _benchmarker.Bench("End", epicPass1, processName);
            _benchmarker.FlushEpic(epicPass1);
            var report = _benchmarker.GetEpicReport(epicName, DateTime.UtcNow.AddMinutes(-1));

            Assert.Single(report);
            Assert.True(report.ContainsKey(epicPass1));
            Assert.Equal(3, report[epicPass1].Count);

            Assert.Equal("Start", report[epicPass1][0].Checkpoint);
            Assert.Equal("Processing", report[epicPass1][1].Checkpoint);
            Assert.Equal("End", report[epicPass1][2].Checkpoint);

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
