using System;

namespace Signals.Aspects.BackgroundProcessing.TaskConfiguration
{
	/// <summary>
	/// Advanced configuration for months
	/// Ex: Days: 10 of every 1 month
	/// </summary>
	public sealed class MonthlyRecurrencePatternConfiguration : RecurrencePatternConfiguration
	{
		/// <summary>
		/// At what month to occur
		/// Ex: Every 2nd month (Jan, March, May)
		/// </summary>
		public int Month
		{
			get => Convert.ToInt32(Value);
			private set => Value = value;
		}

		/// <summary>
		/// At what day to occur
		/// Ex: Occurs every 5th day
		/// </summary>
		public int Day { get; private set; }

		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="value"></param>
		public MonthlyRecurrencePatternConfiguration(int value) : base(PatternType.Month, value)
		{

		}

		/// <summary>
		/// On which day to occur
		/// Ex: On First
		/// </summary>
		/// <param name="day"></param>
		/// <returns></returns>
		public MonthlyRecurrencePatternConfiguration On(int day)
		{
			Day = day;
			return this;
		}

		/// <summary>
		/// Every 1st, 2nd, 3rd ..nth month
		/// Ex: of March
		/// </summary>
		/// <param name="month"></param>
		/// <returns></returns>
		public MonthlyRecurrencePatternConfiguration Every(int month)
		{
			Month = month;
			return this;
		}

		/// <summary>
		/// At what time to occur
		/// </summary>
		/// <param name="hour"></param>
		/// <param name="minute"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public MonthlyRecurrencePatternConfiguration At(int hour, int minute, int second)
		{
			TimePart = new TimeSpan(0, hour, minute, second);
			return this;
		}
	}
}
