using System;
using System.Collections.Generic;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;

namespace Signals.Core.Processing.Input.Http.ModelBinding
{
	/// <summary>
	/// Create a IDto instance from the query headers content
	/// </summary>
	public class FromHeader : BaseModelBinder
	{
		/// <summary>
		/// Bind the HTTP Context Request data to an object
		/// </summary>
		/// <returns></returns>
		public override object Bind(IHttpContextWrapper httpContext)
		{
			var headers = httpContext?.Headers.GetFromRequest();
			if (headers.IsNull())
			{
				this.D("Headers are null. Exit Model Binder.");
				return null;
			}

			var obj = new Dictionary<string, string>();
			foreach (var key in headers.Keys)
			{
				headers.TryGetValue(key, out var value);
				obj[key] = value?.ToString();
			}

			var dto = obj.SerializeJson();
			this.D($"Headers -> Value: {dto}. Exit Model Binder.");

			return dto;
		}
	}
}
