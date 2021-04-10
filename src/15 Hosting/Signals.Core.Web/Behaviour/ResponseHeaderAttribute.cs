using System;
using System.Collections.Generic;

namespace Signals.Core.Web.Behaviour
{
    /// <summary>
    /// Adds http response header
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ResponseHeaderAttribute : Attribute
    {
		/// <summary>
		/// Defined response headers
		/// </summary>
	    public Dictionary<string, string> Headers { get; }

		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public ResponseHeaderAttribute(string key, string value)
        {
			Headers = new Dictionary<string, string>()
			{
				{ key, value }
			};
        }

		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="headers"></param>
		public ResponseHeaderAttribute(params KeyValuePair<string, string>[] headers)
	    {
		    Headers = new Dictionary<string, string>();
		    foreach (var header in headers)
		    {
				if (Headers.ContainsKey(header.Key))
				{
					Headers[header.Key] = header.Value;
				}
				else
				{
					Headers.Add(header.Key, header.Value);
				}
			}
	    }
    }
}
