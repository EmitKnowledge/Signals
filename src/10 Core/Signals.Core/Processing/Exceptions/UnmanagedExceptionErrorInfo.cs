using Signals.Aspects.DI;
using Signals.Aspects.Localization;
using Signals.Core.Common.Exceptions;
using Signals.Core.Configuration;
using System;
using System.Runtime.Serialization;

namespace Signals.Core.Processing.Exceptions
{
    /// <summary>
    /// Application error information
    /// </summary>
    [Serializable]
    [DataContract]
    public class UnmanagedExceptionErrorInfo : IErrorInfo
    {
        /// <summary>
        /// System fault message
        /// </summary>
        [DataMember]
        public string FaultMessage { get; set; }

        /// <summary>
        /// User visible error message. What the user see
        /// </summary>
        [DataMember]
        public string UserVisibleMessage { get; private set; }

		/// <summary>
		/// Exception reason for error
		/// </summary>
		[IgnoreDataMember]
		public Exception Exception { get; private set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="exception"></param>
        public UnmanagedExceptionErrorInfo(Exception exception)
        {
            Exception = exception;

            if (ApplicationConfiguration.Instance.EnableVerbose)
            {
				FaultMessage = ExceptionsExtensions.Extract(exception,
                    ExceptionsExtensions.ExceptionDetails.Type,
                    ExceptionsExtensions.ExceptionDetails.Message,
                    ExceptionsExtensions.ExceptionDetails.Stacktrace);

			}
            
            var localizer = SystemBootstrapper.GetInstance<ILocalizationProvider>();
            UserVisibleMessage = localizer?.Get("Signals_SystemError")?.Value ?? "Server error!";
        }
    }
}