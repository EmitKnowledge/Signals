using System;

namespace Signals.Aspects.BackgroundProcessing.TaskConfiguration
{
	/// <summary>
	/// Advanced configuration for days
	/// </summary>
	public sealed class DailyRecurrencePatternConfiguration : RecurrencePatternConfiguration
	{
		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="value"></param>
		public DailyRecurrencePatternConfiguration(int value) : base(PatternType.Day, value)
		{

		}

		/// <summary>
		/// At what time to occur
		/// </summary>
		/// <param name="hour"></param>
		/// <param name="minute"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public DailyRecurrencePatternConfiguration At(int hour, int minute, int second)
		{
			TimePart = new TimeSpan(0, hour, minute, second);
			return this;
		}
	}
}
