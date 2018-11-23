using NodaTime;
using Signals.Core.Common.Instance;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Signals.Core.Common.Dates
{
    /// <summary>
    /// Helper for date time formatting
    /// </summary>
    public static class DateTimeFormatting
    {
        /// <summary>
        /// Return a list of supported date formats
        /// </summary>
        /// <returns></returns>
        public static List<string> AvailableFormats()
        {
            return new List<string>
            {
                DDMMYYYY(@"/"),
                DDMMYYYY(@"."),
                DDMMYYYY(@"-"),
                MMDDYYYY(@"/"),
                MMDDYYYY(@"."),
                MMDDYYYY(@"-"),
                YYYYMMDD(@"/"),
                YYYYMMDD(@"."),
                YYYYMMDD(@"-"),
            };
        }

        /// <summary>
        /// YYYY MM DD - date format
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string YYYYMMDD(string separator = "-")
        {
            return $@"yyyy{separator}MM{separator}dd";
        }

        /// <summary>
        /// DD MM YYYY - date format
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string DDMMYYYY(string separator = "-")
        {
            return $@"dd{separator}MM{separator}yyyy";
        }

        /// <summary>
        /// MM DD YYYY - date format
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string MMDDYYYY(string separator = "-")
        {
            return $@"MM{separator}dd{separator}yyyy";
        }

        /// <summary>
        /// Return the closest available format for the provided short date pattern and date separator
        /// </summary>
        /// <param name="shortDatePattern"></param>
        /// <param name="dateSeparator"></param>
        /// <returns></returns>
        public static string Match(string shortDatePattern, string dateSeparator)
        {
            if (shortDatePattern.IsNullOrEmpty() ||
               dateSeparator.IsNullOrEmpty()) return YYYYMMDD(dateSeparator);

            var flat = shortDatePattern.ToLower().ToCharArray();

            switch (flat[0])
            {
                case 'd':
                    return DDMMYYYY(dateSeparator);

                case 'm':
                    return MMDDYYYY(dateSeparator);

                case 'y':
                    return YYYYMMDD(dateSeparator);

                default:
                    return YYYYMMDD(dateSeparator);
            }
        }

        /// <summary>
        /// Return the closest available format for the provided short date pattern and date separator
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Find(string format)
        {
            // if no format is provided return the default one
            var existingFormat = AvailableFormats().FirstOrDefault(x => string.Compare(x, format, StringComparison.InvariantCultureIgnoreCase) == 0);
            if (existingFormat.IsNullOrEmpty()) return YYYYMMDD();

            return existingFormat;
        }

        /// <summary>
        /// Return date in ISO 8601 representation format
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToFormat(this Instant dateTime, string format)
        {
            return ToFormat((Instant?)dateTime, format);
        }

        /// <summary>
        /// Return date in ISO 8601 representation format
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToFormat(this Instant? dateTime, string format)
        {
            // if no format is provided return the default one
            var existingFormat = AvailableFormats().FirstOrDefault(x => string.Compare(x, format, StringComparison.InvariantCultureIgnoreCase) == 0);
            if (existingFormat.IsNullOrEmpty()) return YYYYMMDD();

            var formattedDate = dateTime?.ToString(format, CultureInfo.InvariantCulture);
            return formattedDate;
        }
    }
}