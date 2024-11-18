using Signals.Core.Processing.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Signals.Core.Processing.Results
{
	/// <summary>
	/// Process result wrapper
	/// </summary>
	[Serializable]
	[DataContract]
	public partial class VoidResult
	{
		/// <summary>
		/// Indicate that the result is faulted
		/// </summary>
		[DataMember]
		public bool IsFaulted { get; set; }

		/// <summary>
		/// Indicate that the result is not valid due to a system exception
		/// </summary>
		[IgnoreDataMember]
		public bool IsSystemFault { get; set; }

		/// <summary>
		/// Error information related to the result
		/// </summary>
		[DataMember]
		public List<IErrorInfo> ErrorMessages { get; set; }

		/// <summary>
		/// CTOR
		/// </summary>
		[DebuggerStepThrough]
		public VoidResult()
		{
			ErrorMessages = [];
		}

		/// <summary>
		/// Return all errors fault message
		/// </summary>
		/// <returns></returns>
		public string GetFaultMessage()
		{
			var message = string.Join(@" | ", ErrorMessages.Select(x => x.FaultMessage));
			return message;
		}

		/// <summary>
		/// Return all errors user visible message
		/// </summary>
		/// <returns></returns>
		public string GetUserVisibleMessage()
		{
			var message = string.Join(@" | ", ErrorMessages.Select(x => x.UserVisibleMessage));
			return message;
		}

		/// <summary>
		/// Create default faulted results
		/// </summary>
		/// <returns></returns>
		[Obsolete("Use VoidResult -> Fail()")]
		public static VoidResult FaultedResult()
		{
			var faultedResult = new VoidResult
			{
				IsFaulted = true
			};

			return faultedResult;
		}

		/// <summary>
		/// Create default faulted results
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[Obsolete("Use VoidResult -> T Fail<T>()")]
		public static T FaultedResult<T>() where T : VoidResult, new()
		{
			var faultedResult = new T
			{
				IsFaulted = true
			};

			return faultedResult;
		}

		/// <summary>
		/// Create default faulted results
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[Obsolete("Use VoidResult -> T Fail<T>(params IErrorInfo[] errors)")]
		public static T FaultedResult<T>(params IErrorInfo[] errors) where T : VoidResult, new()
		{
			var faultedResult = new T
			{
				IsFaulted = true
			};

			if (errors?.Any() == true)
				faultedResult.ErrorMessages.AddRange(errors);

			return faultedResult;
		}

		/// <summary>
		/// Create default faulted results
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[Obsolete("Use VoidResult -> T Fail<T>(Exception ex)")]
		public static T FaultedResult<T>(Exception ex) where T : VoidResult, new()
		{
			var faultedResult = new T
			{
				IsFaulted = true,
				IsSystemFault = true
			};

			faultedResult.ErrorMessages.Add(new UnmanagedExceptionErrorInfo(ex));

			return faultedResult;
		}

		/// <summary>
		/// Create default faulted results
		/// </summary>
		/// <returns></returns>
		public static VoidResult Fail()
		{
			var faultedResult = new VoidResult
			{
				IsFaulted = true
			};

			return faultedResult;
		}

		/// <summary>
		/// Create default faulted results
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T Fail<T>() where T : VoidResult, new()
		{
			var faultedResult = new T
			{
				IsFaulted = true
			};

			return faultedResult;
		}

		/// <summary>
		/// Create default faulted results
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T Fail<T>(params IErrorInfo[] errors) where T : VoidResult, new()
		{
			var faultedResult = new T
			{
				IsFaulted = true
			};

			if (errors?.Any() == true)
				faultedResult.ErrorMessages.AddRange(errors);

			return faultedResult;
		}

		/// <summary>
		/// Create default faulted results
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T Fail<T>(Exception ex) where T : VoidResult, new()
		{
			var faultedResult = new T
			{
				IsFaulted = true,
				IsSystemFault = true
			};

			faultedResult.ErrorMessages.Add(new UnmanagedExceptionErrorInfo(ex));

			return faultedResult;
		}

		/// <summary>
		/// Empty successful void result
		/// </summary>
		[Obsolete("Use VoidResult -> Ok()")]
		public static VoidResult Default()
		{
			return new VoidResult();
		}

		/// <summary>
		/// Empty successful void result
		/// </summary>
		public static VoidResult Ok()
		{
			return new VoidResult();
		}
	}
}