using System;

namespace Signals.Aspects.BackgroundProcessing.TaskConfiguration
{
	/// <summary>
	/// Every day in weekend
	/// </summary>
	public sealed class WeekendRecurrencePatternConfiguration : RecurrencePatternConfiguration
	{
		/// <summary>
		/// On which days to occur.
		/// Empty means all
		/// </summary>
		public DayOfWeek[] Days { get; }

		/// <summary>
		/// CTOR
		/// </summary>
		public WeekendRecurrencePatternConfiguration() : base(PatternType.Day, 1)
		{
			Days = new[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
		}

		/// <summary>
		/// At what time to occur
		/// </summary>
		/// <param name="hour"></param>
		/// <param name="minute"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public WeekendRecurrencePatternConfiguration At(int hour, int minute, int second)
		{
			TimePart = new TimeSpan(0, hour, minute, second);
			return this;
		}
	}
}
