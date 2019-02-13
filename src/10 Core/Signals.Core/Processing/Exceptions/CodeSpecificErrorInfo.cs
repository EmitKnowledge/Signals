using System;
using System.Net;
using System.Runtime.Serialization;

namespace Signals.Core.Processing.Exceptions
{
    /// <summary>
    /// User specified error info
    /// </summary>
    [Serializable]
    [DataContract]
    public class CodeSpecificErrorInfo : IErrorInfo
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
        /// Http status code sent to the client
        /// </summary>
        [IgnoreDataMember]
        public HttpStatusCode HttpStatusCode { get; private set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="faultMessage"></param>
        /// <param name="userVisibleMessage"></param>
        /// <param name="httpStatusCode"></param>
        public CodeSpecificErrorInfo(string faultMessage, string userVisibleMessage, HttpStatusCode httpStatusCode)
        {
            FaultMessage = faultMessage;
            UserVisibleMessage = userVisibleMessage;
            HttpStatusCode = httpStatusCode;
        }
    }
}
