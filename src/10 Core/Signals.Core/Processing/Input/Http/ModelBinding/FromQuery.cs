using System.Collections.Generic;
using System.Linq;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;

namespace Signals.Core.Processing.Input.Http.ModelBinding
{
	/// <summary>
	/// Create a IDto instance from the query string content
	/// </summary>
	public class FromQuery : BaseModelBinder
	{
		/// <summary>
		/// Bind the HTTP Context Request data to an object
		/// </summary>
		/// <returns></returns>
		public override object Bind(IHttpContextWrapper httpContext)
		{
			var query = httpContext?.Query;
			if (query.IsNull()) return null;

			var obj = new Dictionary<string, string>();
			foreach (var key in query.Keys)
			{
				query.TryGetValue(key, out var value);
				obj[key] = value?.FirstOrDefault();
			}

			var dto = obj.SerializeJson();
			return dto;
		}
	}
}
