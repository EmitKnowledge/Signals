using Microsoft.AspNetCore.Http;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Processing.Input.Http;

namespace Signals.Core.Web.Http
{
    /// <summary>
    /// Wrapper around real session
    /// </summary>
    public class SessionProvider : ISessionProvider
    {
	    /// <summary>
        /// Real http context
        /// </summary>
        private Microsoft.AspNetCore.Http.HttpContext _context;
        
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="context"></param>
        public SessionProvider(Microsoft.AspNetCore.Http.HttpContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Sets session value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Set(string name, object value)
        {
            if (!_context.Session.IsNull())
                _context.Session.SetString(name, value.SerializeJson());
        }
        
        /// <summary>
        /// Removes session value
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            _context.Session?.Remove(name);
        }
        
        /// <summary>
        /// Gets session value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get<T>(string name) where T : class
        {
            var sessionValue = _context.Session?.GetString(name);
            if (sessionValue.IsNull()) return (T)null;
            return sessionValue.Deserialize<T>(SerializationFormat.Json);
        }
    }
}
