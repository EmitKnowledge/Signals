using Signals.Core.Configuration;
using System;
using Signals.Core.Configuration.ConfigurationSegments;

namespace Signals.Core.Processing.Behaviour
{
    /// <summary>
    /// Marks process as critical and notifies by email
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CriticalAttribute : Attribute
    {
        /// <summary>
        /// Notified person, default is <see cref="CriticalConfiguration.OwnerEmail"/>
        /// </summary>
        public string NotificaitonEmail { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public CriticalAttribute()
        {
            NotificaitonEmail = ApplicationConfiguration.Instance?.CriticalConfiguration?.OwnerEmail;
        }
    }
}
