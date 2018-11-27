using System;
using System.Web;

namespace App.Client.Web.Infrastructure.Cookies
{
    public static class CookieHelper
    {
        private static HttpContext Context { get { return HttpContext.Current; } }

        /// <summary>
        /// Set cookie
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="durationInDays"></param>
        /// <param name="isHttpOnly"></param>
        public static void Set(string key, string value, int durationInDays = 1, bool isHttpOnly = false)
        {
            var c = new HttpCookie(key)
            {
                Value = value,
                Expires = DateTime.Now.AddDays(durationInDays),
                HttpOnly = isHttpOnly
            };
            Context.Response.Cookies.Add(c);
        }

        /// <summary>
        /// Get cookie
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            var value = string.Empty;

            var c = Context.Request.Cookies[key];
            return c != null
                    ? Context.Server.HtmlEncode(c.Value).Trim()
                    : value;
        }

        /// <summary>
        /// Check if cookie exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            return Context.Request.Cookies[key] != null;
        }

        /// <summary>
        /// Delete a cookie
        /// </summary>
        /// <param name="key"></param>
        public static void Delete(string key)
        {
            if (Exists(key))
            {
                var c = new HttpCookie(key) { Expires = DateTime.Now.AddDays(-1) };
                Context.Response.Cookies.Add(c);
            }
        }

        /// <summary>
        /// Delete all cookis from request
        /// </summary>
        public static void DeleteAll()
        {
            for (int i = 0; i <= Context.Request.Cookies.Count - 1; i++)
            {
                if (Context.Request.Cookies[i] != null)
                {
                    Delete(Context.Request.Cookies[i].Name);
                }
            }
        }
    }
}