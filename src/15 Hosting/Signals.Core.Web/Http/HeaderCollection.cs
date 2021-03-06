﻿using Signals.Core.Processing.Input.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Core.Web.Http
{
    /// <summary>
    /// Wrapper around real header collection
    /// </summary>
    public class HeaderCollection : IHeaderCollection
    {

#if (NET461)

        /// <summary>
        /// Real http context
        /// </summary>
        private System.Web.HttpContext _context;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="context"></param>
        public HeaderCollection(System.Web.HttpContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add header to response
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddToResponse(string name, string value)
        {
            _context.Response.Headers.Remove(name);
            _context.Response.AddHeader(name, value);
        }

        /// <summary>
        /// Removes all headers from response
        /// </summary>
        public void RemoveAllFromResponse()
        {
            _context.Response.ClearHeaders();
        }

	    /// <summary>
	    /// Gets headers from request
	    /// </summary>
	    /// <returns></returns>
	    public Dictionary<string, object> GetFromRequest()
	    {
		    var headers = new Dictionary<string, object>();
		    foreach (var key in _context.Request.Headers.AllKeys)
		    {
			    var value = _context.Request.Headers[key];
			    headers.Add(key, value);
		    }
		    return headers;
	    }

		/// <summary>
		/// Get headers from response
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, object> GetFromResponse()
	    {
			var headers = new Dictionary<string, object>();
			foreach (var key in _context.Response.Headers.AllKeys)
		    {
			    var value = _context.Response.Headers[key];
			    headers.Add(key, value);
		    }
			return headers;
		}

		/// <summary>
		/// Get headers from response
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetFromResponse(string name)
        {
            return _context.Response.Headers[name];
        }

        /// <summary>
        /// Gets headers from request
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetFromRequest(string name)
        {
            return _context.Request.Headers[name];
        }

#else
        
        /// <summary>
        /// Real http context
        /// </summary>
        private Microsoft.AspNetCore.Http.HttpContext _context;
        
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="context"></param>
        public HeaderCollection(Microsoft.AspNetCore.Http.HttpContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Add header to response
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddToResponse(string name, string value)
        {
            _context.Response.Headers.Remove(name);
            _context.Response.Headers.Add(name, value);
        }

        /// <summary>
        /// Removes all headers from response
        /// </summary>
        public void RemoveAllFromResponse()
        {
            _context.Response.Headers.Clear();
        }

	    /// <summary>
	    /// Gets headers from request
	    /// </summary>
	    /// <returns></returns>
	    public Dictionary<string, object> GetFromRequest()
	    {
		    var headers = new Dictionary<string, object>();
		    foreach (var header in _context.Request.Headers)
		    {
			    headers.Add(header.Key, header.Value);
		    }
		    return headers;
	    }

		/// <summary>
		/// Get headers from response
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, object> GetFromResponse()
	    {
			var headers = new Dictionary<string, object>();
			foreach (var header in _context.Response.Headers)
		    {
			    headers.Add(header.Key, header.Value);
			}
			return headers;
		}

        /// <summary>
        /// Get headers from response
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetFromResponse(string name)
        {
            return _context.Response.Headers[name];
        }

        /// <summary>
        /// Gets headers from request
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetFromRequest(string name)
        {
            return _context.Request.Headers[name];
        }

#endif

	}
}
