using Signals.Aspects.Benchmarking.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Aspects.Benchmarking.Database.Configurations
{
    /// <summary>
    /// Benchmarking configuration for database
    /// </summary>
    public class DatabaseBenchmarkingConfiguration : IBenchmarkingConfiguration
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public DatabaseBenchmarkingConfiguration()
        {
            TableName = "BenchmarkingEntry";
        }

        /// <summary>
        /// Represents the connection string to the database with benchmarking table
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Represents the benchmarking table name in the database
        /// </summary>
        public string TableName { get; set; }
    }
}
