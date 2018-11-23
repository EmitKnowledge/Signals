using System;

namespace Signals.Aspects.BackgroundProcessing.TaskConfiguration
{
	/// <summary>
	/// Every day in without weeknds
	/// </summary>
	public sealed class WorkdayRecurrencePatternConfiguration : RecurrencePatternConfiguration
	{
		/// <summary>
		/// On which days to occur.
		/// Empty means all
		/// </summary>
		public DayOfWeek[] Days { get; }

		/// <summary>
		/// CTOR
		/// </summary>
		public WorkdayRecurrencePatternConfiguration() : base(PatternType.Day, 1)
		{
			Days = new[]
			{
				DayOfWeek.Monday,
				DayOfWeek.Tuesday,
				DayOfWeek.Wednesday,
				DayOfWeek.Thursday,
				DayOfWeek.Friday
			};
		}

		/// <summary>
		/// At what time to occur
		/// </summary>
		/// <param name="hour"></param>
		/// <param name="minute"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public WorkdayRecurrencePatternConfiguration At(int hour, int minute, int second)
		{
			TimePart = new TimeSpan(0, hour, minute, second);
			return this;
		}
	}
}
