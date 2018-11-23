using Newtonsoft.Json;
using Signals.Aspects.Bootstrap;
using Signals.Aspects.Localization;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Processing.Specifications;
using System;
using System.Runtime.Serialization;

namespace Signals.Core.Processing.Exceptions
{
    [Serializable]
    [DataContract]
    internal class SpecificationErrorInfo : IErrorInfo
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
        public SpecificationErrorInfo(SpecificationResult result)
        {
            if (!result.IsValid)
            {
                FaultMessage = result.Input.IsNull() ?
                    $"Specificaiton {result.SpecificationType.Name} failed" :
                    $"Specificaiton {result.SpecificationType.Name} failed for input {result.Input?.GetType().Name} with payload {result.Input.SerializeJson()}";

                var localizer = SystemBootstrapper.GetInstance<ILocalizationProvider>();
                UserVisibleMessage = localizer?.Get(result.SpecificationType.Name)?.Value;
            }
        }
    }
}