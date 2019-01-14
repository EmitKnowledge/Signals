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
        public static DateTime DatePart(this DateTime dateTime)
        {
            var date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 00, 00, 00);
            return date;
        }

        /// <summary>
        /// Convert the provided date to UNIX ticks
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long Ticks(this DateTime dateTime)
        {
            return dateTime.Ticks - new DateTime(1970, 1, 1).Ticks;
        }
    }
}