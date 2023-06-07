using Signals.Core.Common.Instance;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Core.Web.Http
{
    /// <summary>
    /// Wrapper around real form collection
    /// </summary>
    public class FormCollection : Dictionary<string, object>, Signals.Core.Processing.Input.Http.IFormCollection
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Return all of the available keys</summary>
        public new ICollection<string> Keys => base.Keys;

        /// <summary>
        /// Real http context
        /// </summary>
        private Microsoft.AspNetCore.Http.HttpContext _context;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="context"></param>
        public FormCollection(Microsoft.AspNetCore.Http.HttpContext context)
        {
            _context = context;
            if (!_context.Request.HasFormContentType) return;
            var form = _context.Request?.Form;
            if (form.IsNull()) return;
            foreach (var key in form.Keys)
            {
                var value = form[key];
                if (value.Count == 1)
                    this.Add(key, value.First());
                else
                    this.Add(key, value);
            }
        }
    }
}
