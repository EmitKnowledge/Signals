using System;

namespace Signals.Aspects.BackgroundProcessing.TaskConfiguration
{
	/// <summary>
	/// Advanced configuration for time
	/// Ex: Run every 5 seconds;
	///     Run every 5 minutes and 30 seconds
	///		Run every 1 hour and 30 minutes
	/// </summary>
	public sealed class TimePartRecurrencePatternConfiguration : RecurrencePatternConfiguration
	{
		/// <summary>
		/// CTOR
		/// </summary>
		public TimePartRecurrencePatternConfiguration(TimeSpan timePart) : base(PatternType.Second, 1)
		{
			TimePart = new TimeSpan(0, timePart.Hours, timePart.Minutes, timePart.Seconds);
			Value = (int)TimePart.TotalSeconds;
		}
	}
}
