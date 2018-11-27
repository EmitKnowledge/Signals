using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Signals.Core.Processing.Results
{
    /// <summary>
    /// List result wrapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DataContract]
    public class ListResult<T> : VoidResult
    {
        /// <summary>
        /// Hold a value of total records count.
        /// Filled to indicate the total amount of data in pagination process
        /// </summary>
        [DataMember]
        public int? TotalCount { get; set; }

        /// <summary>
        /// Hold a value of total records count.
        /// Filled to indicate the total amount of data in pagination process
        /// </summary>
        [DataMember]
        public int Count => Result.Count;

        /// <summary>
        /// Real result
        /// </summary>
        [DataMember]
        public List<T> Result { get; private set; }

        /// <summary>
        /// CTOR
        /// </summary>
        [DebuggerStepThrough]
        public ListResult()
        {
            Result = new List<T>();
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="result"></param>
        /// <param name="totalCount"></param>
        public ListResult(List<T> result = null, int? totalCount = null)
        {
            Result = result ?? new List<T>();
            TotalCount = totalCount;
        }

        /// <summary>
        /// Wrap result in list result
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator ListResult<T>((List<T> list, int totalCount) result)
        {
            return new ListResult<T>(result.list, result.totalCount);
        }

        /// <summary>
        /// Wrap result in list result
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator ListResult<T>(List<T> result)
        {
            return new ListResult<T>(result);
        }

        /// <summary>
        /// Wrap result in method result
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator List<T>(ListResult<T> result)
        {
            return result.Result ?? new List<T>();
        }
    }
}