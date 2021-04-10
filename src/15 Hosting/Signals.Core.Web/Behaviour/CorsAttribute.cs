using System;
using System.Collections.Generic;

namespace Signals.Core.Web.Behaviour
{
    /// <summary>
    /// CORS header
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CorsAttribute : ResponseHeaderAttribute
    {
		/// <summary>
		/// CTOR allow for multiple [allowOrigin] origin, multiple [allowMethods] methods and multiple [allowHeaders] headers
		/// </summary>
		public CorsAttribute(string[] allowOrigin, 
							 string[] allowMethods, 
							 string[] allowHeaders) : 
	        base(
					new KeyValuePair<string, string>(@"Access-Control-Allow-Origin", string.Join(", ", allowOrigin)),
					new KeyValuePair<string, string>(@"Access-Control-Allow-Methods", string.Join(", ", allowMethods)),
					new KeyValuePair<string, string>(@"Access-Control-Allow-Headers", string.Join(", ", allowHeaders))
		        )
        {

		}

	    /// <summary>
	    /// CTOR allow for multiple [allowOrigin] origin, * methods and * headers
	    /// </summary>
		public CorsAttribute(string[] allowOrigin) :
		    base(
			    new KeyValuePair<string, string>(@"Access-Control-Allow-Origin", string.Join(", ", allowOrigin)),
			    new KeyValuePair<string, string>(@"Access-Control-Allow-Methods", "*"),
			    new KeyValuePair<string, string>(@"Access-Control-Allow-Headers", "*")
		    )
	    {

	    }

		/// <summary>
		/// CTOR allow for single [allowOrigin] origin, * methods and * headers
		/// </summary>
		public CorsAttribute(string allowOrigin) :
		    base(
			    new KeyValuePair<string, string>(@"Access-Control-Allow-Origin", allowOrigin),
			    new KeyValuePair<string, string>(@"Access-Control-Allow-Methods", "*"),
			    new KeyValuePair<string, string>(@"Access-Control-Allow-Headers", "*")
		    )
	    {

	    }

	    /// <summary>
	    /// CTOR allow for * origin, * methods and * headers
	    /// </summary>
	    public CorsAttribute() :
		    base(
			    new KeyValuePair<string, string>(@"Access-Control-Allow-Origin", "*"),
			    new KeyValuePair<string, string>(@"Access-Control-Allow-Methods", "*"),
			    new KeyValuePair<string, string>(@"Access-Control-Allow-Headers", "*")
		    )
	    {

	    }
	}
}
