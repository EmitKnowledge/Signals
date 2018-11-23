using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Signals.Core.Processing.Results
{
    [Serializable]
    [DataContract]
    public class MethodResult<T> : VoidResult
    {
        /// <summary>
        /// The execution result
        /// </summary>
        [DataMember]
        public T Result { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        [DebuggerStepThrough]
        public MethodResult() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="result"></param>
        public MethodResult(T result)
        {
            Result = result;
        }

        /// <summary>
        /// Wrap result in method result
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator MethodResult<T>(T result)
        {
            return new MethodResult<T>(result);
        }

        /// <summary>
        /// Wrap result in method result
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator T(MethodResult<T> result)
        {
            return result.Result != null ? result.Result : default(T);
        }
    }
}