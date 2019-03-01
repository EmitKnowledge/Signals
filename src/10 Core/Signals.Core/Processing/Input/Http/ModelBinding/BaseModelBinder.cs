using System;

namespace Signals.Core.Processing.Input.Http.ModelBinding
{
	/// <summary>
	/// Represents base model binder
	/// </summary>
	public abstract class BaseModelBinder
	{
		/// <summary>
		/// Bind the HTTP Context Request data to an object
		/// </summary>
		/// <returns></returns>
		public abstract object Bind(IHttpContextWrapper httpContext);
	}
}
