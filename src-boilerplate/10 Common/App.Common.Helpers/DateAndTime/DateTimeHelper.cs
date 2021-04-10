using NodaTime;
using System.Collections.Generic;

namespace App.Common.Helpers.DateAndTime
{
    /// <summary>
    /// Extensions for date time
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Split date range into chunks
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="dayChunkSize"></param>
        /// <returns></returns>
        public static IEnumerable<(Instant From, Instant To)> SplitDateRange(Instant start, Instant end, int dayChunkSize)
        {
            Instant chunkEnd;
            while ((chunkEnd = start + Duration.FromDays(dayChunkSize)) < end)
            {
                yield return (start, chunkEnd);
                start = chunkEnd;
            }
            yield return (start, end);
        }
    }
}