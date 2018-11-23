using Signals.Aspects.Bootstrap;
using Signals.Aspects.Localization;
using System;
using System.Runtime.Serialization;

namespace Signals.Core.Processing.Exceptions
{
    [Serializable]
    [DataContract]
    public class UnmanagedExceptionErrorInfo : IErrorInfo
    {
        /// <summary>
        /// System fault message
        /// </summary>
        [IgnoreDataMember]
        public string FaultMessage { get; set; }

        /// <summary>
        /// User visible error message. What the user see
        /// </summary>
        [DataMember]
        public string UserVisibleMessage { get; private set; }

        /// <summary>
        /// Exception reason for error
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="ex"></param>
        public UnmanagedExceptionErrorInfo(Exception ex)
        {
            Exception = ex;
            FaultMessage = ex.Message;

            var localizer = SystemBootstrapper.GetInstance<ILocalizationProvider>();
            UserVisibleMessage = localizer?.Get("SystemError")?.Value ?? "Something happened!";
        }
    }
}