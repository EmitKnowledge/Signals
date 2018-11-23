using NodaTime;
using System;

namespace Signals.Core.Common.Dates
{
    /// <summary>
    /// Helper for manipulating date time
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Remove the date part of the proided value
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static Instant DatePart(this Instant dateTime)
        {
            var utc = dateTime.InUtc();
            var date = new DateTime(utc.Year, utc.Month, utc.Day, 00, 00, 00);
            return Instant.FromDateTimeUtc(date);
        }

        /// <summary>
        /// Convert the provided date to UNIX ticks
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long Ticks(this DateTime dateTime)
        {
            return Instant.FromDateTimeUtc(dateTime).ToUnixTimeTicks();
        }
    }
}