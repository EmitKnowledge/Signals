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
            _benchmarker = new Benchmarker(_databaseConfiguration);

            // Clear the database
            ClearDb();
        }

        [Fact]
        public void Test1()
        {
            Guid epicPass1 = Guid.NewGuid();
            string epicName = "My epic";

            _benchmarker.StartEpic(epicPass1, epicName);
            _benchmarker.Bench("Start", epicPass1);
            Thread.Sleep(100);
            _benchmarker.Bench("Processing", epicPass1);
            Thread.Sleep(100);
            _benchmarker.Bench("End", epicPass1);
            _benchmarker.FlushEpic(epicPass1);
            var report = _benchmarker.GetEpicReport(epicName);

            Assert.Single(report);
            Assert.True(report.ContainsKey(epicPass1));
            Assert.Equal(3, report[epicPass1].Count);

            Assert.Equal("Start", report[epicPass1][0].Checkpoint);
            Assert.Equal("Processing", report[epicPass1][1].Checkpoint);
            Assert.Equal("End", report[epicPass1][2].Checkpoint);

        }

        private void ClearDb()
        {
            using (var conn = new SqlConnection(_databaseConfiguration.ConnectionString))
            {
                var sql = $"DELETE FROM [{_databaseConfiguration.TableName}]";
                conn.Open();
                new SqlCommand(sql, conn).ExecuteNonQuery();
            }
        }
    }
}
