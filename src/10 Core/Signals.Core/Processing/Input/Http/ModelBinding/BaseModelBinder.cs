using System;

namespace Signals.Core.Processing.Input.Http.ModelBinding
{
	/// <summary>
	/// Represents base model binder
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public abstract class BaseModelBinder : Attribute
	{
		/// <summary>
		/// Bind the HTTP Context Request data to an object
		/// </summary>
		/// <returns></returns>
		public abstract object Bind(IHttpContextWrapper httpContext);
	}
}
