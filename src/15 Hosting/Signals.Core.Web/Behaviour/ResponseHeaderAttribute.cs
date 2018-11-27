using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.Core.Web.Behaviour
{
    /// <summary>
    /// Adds http response header
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ResponseHeaderAttribute : Attribute
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public ResponseHeaderAttribute(string key, string value)
        {
            Name = key;
            Value = value;
        }

        /// <summary>
        /// Header name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Header value
        /// </summary>
        public string Value { get; set; }
    }
}
