﻿using Signals.Core.Common.Instance;

namespace Signals.Core.Processing.Input.Http.ModelBinding
{
	/// <summary>
	/// Create a IDto instance from the body content
	/// </summary>
	public class FromBody : BaseModelBinder
	{
		/// <summary>
		/// Bind the HTTP Context Request data to an object
		/// </summary>
		/// <returns></returns>
		public override object Bind(IHttpContextWrapper httpContext)
		{
			var body = httpContext?.Body?.Value;
			if (body.IsNullOrEmpty()) return null;
			return body;
		}
	}
}
