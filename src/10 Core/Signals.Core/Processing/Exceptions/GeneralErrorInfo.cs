using Signals.Aspects.DI;
using Signals.Aspects.Localization;
using System;
using System.Runtime.Serialization;

namespace Signals.Core.Processing.Exceptions
{
    /// <summary>
    /// User specified error info
    /// </summary>
    [Serializable]
    [DataContract]
    public class GeneralErrorInfo : IErrorInfo
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
        /// CTOR
        /// </summary>
        /// <param name="faultMessage"></param>
        /// <param name="userVisibleMessage"></param>
        public GeneralErrorInfo(string faultMessage, string userVisibleMessage)
        {
            FaultMessage = faultMessage;
            UserVisibleMessage = userVisibleMessage;
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="faultMessage"></param>
        /// <param name="translateFaultMessage"></param>
        public GeneralErrorInfo(string faultMessage, bool translateFaultMessage = true)
        {
            var localizer = SystemBootstrapper.GetInstance<ILocalizationProvider>();

            FaultMessage = faultMessage;
            UserVisibleMessage = translateFaultMessage ? localizer?.Get(faultMessage)?.Value ?? null : null;
        }
    }
}
