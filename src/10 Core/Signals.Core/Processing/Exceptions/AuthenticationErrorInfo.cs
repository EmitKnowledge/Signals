using System;
using System.Runtime.Serialization;

namespace Signals.Core.Processing.Exceptions
{
    /// <summary>
    /// Error info generated from failed authentication
    /// </summary>
    [Serializable]
    [DataContract]
    public class AuthenticationErrorInfo : IErrorInfo
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
        public AuthenticationErrorInfo()
        {
            UserVisibleMessage = FaultMessage = "Authentication failed";
        }
    }
}
