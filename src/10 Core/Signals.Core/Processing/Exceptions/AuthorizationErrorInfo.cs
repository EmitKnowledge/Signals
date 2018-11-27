using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;

namespace Signals.Core.Processing.Exceptions
{
    /// <summary>
    /// Error info generated from failed authorization
    /// </summary>
    [Serializable]
    [DataContract]
    public class AuthorizationErrorInfo : IErrorInfo
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
        public AuthorizationErrorInfo()
        {
            FaultMessage = "Authorization failed";
        }
    }
}
