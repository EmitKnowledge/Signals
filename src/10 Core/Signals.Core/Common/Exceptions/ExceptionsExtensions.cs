using System;
using System.Linq;
using System.Text;

namespace Signals.Core.Common.Exceptions
{
    /// <summary>
    /// Helper for extracting information from exceptions
    /// </summary>
    public static class ExceptionsExtensions
    {
        /// <summary>
        /// Extract all messages from exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string Extract(this Exception ex)
        {
            return ex.InnerException == null ? ex.Message : ex.Message + " --> " + Extract(ex.InnerException);
        }

        /// <summary>
        /// Pack exception information into a string
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static string Extract(this Exception exception, params ExceptionDetails[] details)
        {
            var logType = details?.Contains(ExceptionDetails.Type) == true;
            var logMessage = details?.Contains(ExceptionDetails.Message) == true;
            var logStacktrace = details?.Contains(ExceptionDetails.Stacktrace) == true;

            var e = exception;
            var s = new StringBuilder();

            while (e != null)
            {
                if (logType)
                {
                    s.Append($"Exception type --> {e.GetType().FullName} ");
                }
                if (logMessage)
                {
                    s.Append($"Message --> {e.Message} ");
                }
                if (logStacktrace)
                {
                    s.Append($"Stacktrace --> {e.StackTrace} ");
                }

                e = e.InnerException;
            }

            return s.ToString();
        }

        /// <summary>
        /// Represent the type of information that can be retrieved from the exception
        /// </summary>
        public enum ExceptionDetails
        {
            Type,
            Message,
            Stacktrace
        }
    }
}