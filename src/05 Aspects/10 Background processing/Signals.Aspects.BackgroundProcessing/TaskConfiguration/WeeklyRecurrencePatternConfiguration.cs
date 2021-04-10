using System;

namespace Signals.Aspects.BackgroundProcessing.TaskConfiguration
{
	/// <summary>
	/// Advanced configuration for weeks
	/// </summary>
	public sealed class WeeklyRecurrencePatternConfiguration : RecurrencePatternConfiguration
	{
		/// <summary>
		/// On which days to occur.
		/// Empty means all
		/// </summary>
		public DayOfWeek Day { get; private set; }

		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="value"></param>
		public WeeklyRecurrencePatternConfiguration(int value) : base(PatternType.Week, value)
		{
			Day = DayOfWeek.Monday;
		}

        /// <summary>
        /// On whicn days to occur
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public WeeklyRecurrencePatternConfiguration On(DayOfWeek day)
		{
			Day = day;
			return this;
		}

		/// <summary>
		/// At what time to occur
		/// </summary>
		/// <param name="hour"></param>
		/// <param name="minute"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public WeeklyRecurrencePatternConfiguration At(int hour, int minute, int second)
		{
			TimePart = new TimeSpan(0, hour, minute, second);
			return this;
		}
	}
}
