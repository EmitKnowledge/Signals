using Newtonsoft.Json;
using Signals.Aspects.DI;
using Signals.Aspects.Localization;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Processing.Specifications;
using System;
using System.Runtime.Serialization;

namespace Signals.Core.Processing.Exceptions
{
    /// <summary>
    /// Error info when specification fails
    /// </summary>
    [Serializable]
    [DataContract]
    public class SpecificationErrorInfo : IErrorInfo
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
        /// <param name="specificaitonResult"></param>
        public SpecificationErrorInfo(SpecificationResult specificaitonResult)
        {
            if (!specificaitonResult.IsValid)
            {
                FaultMessage = specificaitonResult.Input.IsNull() ?
                    $"Specificaiton {specificaitonResult.SpecificationType.Name} failed" :
                    $"Specificaiton {specificaitonResult.SpecificationType.Name} failed for input {specificaitonResult.Input?.GetType().Name} with payload {specificaitonResult.Input.SerializeJson()}";

                var localizer = SystemBootstrapper.GetInstance<ILocalizationProvider>();
                var specificaitonName = specificaitonResult.SpecificationType.Name;
                var parametarlessName = specificaitonName.Split('`')[0];
                UserVisibleMessage = localizer?.Get(parametarlessName)?.Value;
            }
        }
    }
}