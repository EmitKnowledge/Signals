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
			return httpContext?.Query?.SerializeJson();
		}
	}
}
