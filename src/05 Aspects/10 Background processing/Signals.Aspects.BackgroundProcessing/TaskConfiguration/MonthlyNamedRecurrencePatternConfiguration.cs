using System;

namespace Signals.Aspects.BackgroundProcessing.TaskConfiguration
{

	/// <summary>
	/// Advanced configuration for months
	/// Ex: Second Wed. of every 1 month
	/// </summary>
	public sealed class MonthlyNamedRecurrencePatternConfiguration : RecurrencePatternConfiguration
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
		/// Ex: Occurs every Wed.
		/// </summary>
		public DayOfWeek Day { get; private set; }

		/// <summary>
		/// They order of the day in the month.
		/// Ex: Every second Monday in the month
		/// </summary>
		public DayInMonth Order { get; private set; }

		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="value"></param>
		public MonthlyNamedRecurrencePatternConfiguration(int value) : base(PatternType.Month, value)
		{

		}

		/// <summary>
		/// On which day to occur
		/// Ex: On First Monday in month
		/// </summary>
		/// <param name="dayInMonth"></param>
		/// <param name="day"></param>
		/// <returns></returns>
		public MonthlyNamedRecurrencePatternConfiguration On(DayInMonth dayInMonth, DayOfWeek day)
		{
			Order = dayInMonth;
			Day = day;
			return this;
		}

		/// <summary>
		/// Every 1st, 2nd, 3rd ..nth month
		/// Ex: of March
		/// </summary>
		/// <param name="month"></param>
		/// <returns></returns>
		public MonthlyNamedRecurrencePatternConfiguration Every(int month)
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
		public MonthlyNamedRecurrencePatternConfiguration At(int hour, int minute, int second)
		{
			TimePart = new TimeSpan(0, hour, minute, second);
			return this;
		}

		/// <summary>
		/// Order of the day in the month
		/// </summary>
		public enum DayInMonth
		{
            /// <summary>
            /// First
            /// </summary>
			First,

            /// <summary>
            /// Second
            /// </summary>
			Second,

            /// <summary>
            /// Third
            /// </summary>
			Third,

            /// <summary>
            /// Fourth
            /// </summary>
			Fourth,

            /// <summary>
            /// Last
            /// </summary>
			Last
		}
	}
}
