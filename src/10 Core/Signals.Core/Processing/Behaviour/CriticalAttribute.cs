using Signals.Core.Common.Instance;
using Signals.Core.Configuration;
using Signals.Core.Configuration.ConfigurationSegments;
using System;

namespace Signals.Core.Processing.Behaviour
{
    /// <summary>
    /// Marks process as critical and notifies by email
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CriticalAttribute : Attribute
    {
        /// <summary>
        /// Comma separated notified person emails, default is <see cref="CriticalConfiguration.Emails"/>
        /// </summary>
        public string NotificaitonEmails { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public CriticalAttribute()
        {
	        if (!NotificaitonEmails.IsNullOrEmpty())
	        {
		        this.D("No notification email has been found.");
                return;
	        }

            var emails = ApplicationConfiguration.Instance?.CriticalConfiguration?.Emails;
            if (emails.IsNullOrHasZeroElements())
            {
	            this.D("No notification email has been found.");
	            return;
            }

            var emailList = string.Join(",", emails);
            NotificaitonEmails = emailList;
            this.D($"Notification emails: {emailList} configured.");
        }
    }
}
