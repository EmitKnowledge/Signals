using System;
using System.Collections.Generic;

namespace Signals.Aspects.Benchmarking
{
	/// <summary>
	/// Represents an Epic report related to banchmarks
	/// </summary>
	public class EpicsReport
	{
		/// <summary>
		/// Represents the related benhamrking entries associted with the epic
		/// </summary>
		public List<EpicReport> EpicReports { get; internal set; }

		/// <summary>
		/// CTOR
		/// </summary>
		public EpicsReport()
		{
			EpicReports = new List<EpicReport>();
		}
	}

	/// <summary>
	/// Represents an Epic report related to banchmarks
	/// </summary>
	public class EpicReport
	{
		/// <summary>
		/// Represents the id of the epic
		/// </summary>
		public Guid EpicId { get; internal set; }

		/// <summary>
		/// Represents the related benhamrking entries associted with the epic
		/// </summary>
		public List<BenchmarkEntry> BenchmarkEntries { get; internal set; }

		/// <summary>
		/// CTOR
		/// </summary>
		public EpicReport()
		{
			BenchmarkEntries = new List<BenchmarkEntry>();
		}
	}
}
