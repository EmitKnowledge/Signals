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
            [EnumMember]
            Second,
            [EnumMember]
            Minute,
            [EnumMember]
            Hour,
            [EnumMember]
            Day,
            [EnumMember]
            Week,
            [EnumMember]
            Month,
            [EnumMember]
            Year
        }
    }
}
