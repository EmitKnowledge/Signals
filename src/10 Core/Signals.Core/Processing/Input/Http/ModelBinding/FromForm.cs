using System.Collections.Generic;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;

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
			if (form.IsNull())
			{
				this.D("Form is null. Exit Model Binder.");
				return null;
			}

			var obj = new Dictionary<string, string>();
			foreach (var key in form.Keys)
			{
				form.TryGetValue(key, out var value);
				obj[key] = value?.ToString();
			}

            var dto = obj.SerializeJson();
            this.D($"Form -> Value: {dto}. Exit Model Binder.");

			return dto;
		}
	}
}
