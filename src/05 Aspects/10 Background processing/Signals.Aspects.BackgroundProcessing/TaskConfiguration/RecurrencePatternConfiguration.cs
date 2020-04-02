using System;
using System.Runtime.Serialization;

namespace Signals.Aspects.BackgroundProcessing.TaskConfiguration
{
    /// <summary>
    /// Represents a recurrence pattern configuration for configuring the recurring profile
    /// </summary>
    public abstract class RecurrencePatternConfiguration
    {
        /// <summary>
        /// Type of pattern
        /// </summary>
        public PatternType Type { get; private set; }

        /// <summary>
        /// Value of the pattern
        /// </summary>
        public int Value { get; protected set; }

        /// <summary>
        /// Represents the timepart
        /// </summary>
        public TimeSpan TimePart { get; protected set; }

		/// <summary>
		/// Indicates if the process should run now and then follow the recurrence pattern
		/// </summary>
	    public bool RunNow { get; protected set; }

		/// <summary>
		/// CTOR
		/// </summary>
		internal RecurrencePatternConfiguration()
        {
            TimePart = new TimeSpan();
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        internal RecurrencePatternConfiguration(PatternType type, int value)
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Recurrance interval pattern
        /// </summary>
        [Serializable, DataContract]
        public enum PatternType
        {
            /// <summary>
            /// Second
            /// </summary>
            [EnumMember]
            Second,

            /// <summary>
            /// Minute
            /// </summary>
            [EnumMember]
            Minute,

            /// <summary>
            /// Hour
            /// </summary>
            [EnumMember]
            Hour,

            /// <summary>
            /// Day
            /// </summary>
            [EnumMember]
            Day,
            
            /// <summary>
            /// Week
            /// </summary>
            [EnumMember]
            Week,

            /// <summary>
            /// Month
            /// </summary>
            [EnumMember]
            Month,

            /// <summary>
            /// Year
            /// </summary>
            [EnumMember]
            Year
        }
    }
}
