using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.Core.Web.Behaviour
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ResponseHeaderAttribute : Attribute
    {
        public ResponseHeaderAttribute(string key, string value)
        {
            Name = key;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
