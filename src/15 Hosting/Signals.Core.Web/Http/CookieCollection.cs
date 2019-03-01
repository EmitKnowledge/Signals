using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Processing.Input.Http;
using System;

namespace Signals.Core.Web.Http
{
    /// <summary>
    /// Wrapper around real cookie collection
    /// </summary>
    public class CookieCollection : ICookieCollection
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
        public CookieCollection(System.Web.HttpContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add cookie to response
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        public void Add(string name, object value, DateTime expiration)
        {
            var cookie = new System.Web.HttpCookie(name, value.SerializeJson())
            {
                Expires = expiration,
                HttpOnly = true
            };

            _context.Response.SetCookie(cookie);
        }

        /// <summary>
        /// Remove cookie from response
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            if (!_context.Response.Cookies[name].IsNull())
                _context.Response.Cookies[name].Expires = DateTime.Now.AddDays(-1);
        }

        /// <summary>
        /// Return cookie by name from request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get<T>(string name) where T : class
        {
            var cookie = _context.Request.Cookies[name];
            if (cookie == null) return null;

            if (typeof(T) == typeof(string))
            {
                return cookie.Value as T;
            }

            return cookie.Value.Deserialize<T>(SerializationFormat.Json);
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
        public CookieCollection(Microsoft.AspNetCore.Http.HttpContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add cookie to response
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        public void Add(string name, object value, DateTime expiration)
        {
            var options = new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = expiration,
                HttpOnly = true,
                IsEssential = true
            };
            _context.Response.Cookies.Append(name, value is string ? value.ToString() : value.SerializeJson(), options);
        }

        /// <summary>
        /// Remove cookie from response
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            _context.Response.Cookies.Delete(name);
        }

        /// <summary>
        /// Return cookie by name from request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get<T>(string name) where T : class
        {
            var value = _context.Request.Cookies.ContainsKey(name) ? _context.Request.Cookies[name] : null;
            if (value.IsNullOrEmpty()) return (T)null;

            if (typeof(T) == typeof(string))
            {
                return value as T;
            }

            return value.Deserialize<T>(SerializationFormat.Json);
        }

#endif

    }
}
