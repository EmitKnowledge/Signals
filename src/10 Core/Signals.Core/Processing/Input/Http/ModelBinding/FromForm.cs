using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Signals.Core.Common.Instance;

namespace Signals.Core.Processing.Input.Http.ModelBinding
{
	/// <summary>
	/// Create a IDto instance from the form content
	/// </summary>
	public class FromForm : BaseModelBinder
	{
		/// <summary>
		/// Bind the HTTP Context Request data to an object
		/// </summary>
		/// <returns></returns>
		public override object Bind(IHttpContextWrapper httpContext)
		{
			var form = httpContext?.Form;
			if (form.IsNull()) return null;

			var obj = new JObject();
			foreach (var key in form.Keys)
			{
				form.TryGetValue(key, out var value);
				obj[key] = value?.ToString();
			}

			var dto = obj.ToString(Formatting.None);
			return dto;
		}
	}
}
