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

    }
}
