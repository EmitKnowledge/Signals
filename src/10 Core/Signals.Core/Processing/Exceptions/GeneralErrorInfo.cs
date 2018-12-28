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
    }
}
