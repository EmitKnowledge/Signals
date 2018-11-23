using Signals.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Core.Processing.Behaviour
{
    /// <summary>
    /// Marks process as critical and notifies by email
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CriticalAttribute : Attribute
    {
        /// <summary>
        /// Notified person, default is <see cref="ApplicationInformation.OwnerEmail"/>
        /// </summary>
        public string NotificaitonEmail { get; set; }

        public CriticalAttribute()
        {
            NotificaitonEmail = ApplicationInformation.Instance?.CriticalConfiguration?.OwnerEmail;
        }
    }
}
