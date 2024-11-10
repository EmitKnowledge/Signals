using System;
using System.Collections.Generic;

namespace Signals.Aspects.Benchmarking
{
	/// <summary>
	/// Represents an Correlation report related to banchmarks
	/// </summary>
	public class BenchmarkReport
	{
		/// <summary>
		/// Represents the related benhamrking entries associted with the correlation
		/// </summary>
		public List<CorrelationReport> CorrelationReports { get; internal set; }

		/// <summary>
		/// CTOR
		/// </summary>
		public BenchmarkReport()
		{
			CorrelationReports = new List<CorrelationReport>();
		}
	}

	/// <summary>
	/// Represents an Correlation report related to banchmarks
	/// </summary>
	public class CorrelationReport
	{
		/// <summary>
		/// Represents the correlation id. Used to identify a group of related activities
		/// </summary>
		public Guid CorrelationId { get; internal set; }

		/// <summary>
		/// Represents the related benhamrking entries associted with the correlation
		/// </summary>
		public List<BenchmarkEntry> BenchmarkEntries { get; internal set; }

		/// <summary>
		/// CTOR
		/// </summary>
		public CorrelationReport(Guid correlationId)
		{
			CorrelationId = correlationId;
			BenchmarkEntries = new List<BenchmarkEntry>();
		}
	}
}
