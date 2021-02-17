using App.Domain.Entities.Emails;
using System.Collections.Generic;

namespace App.Domain.Processes.Emails.Dtos.Request
{
    public class SendEmailRequestDto
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public SendEmailRequestDto()
        {
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();
            Log = true;
        }

        /// <summary>
        /// Receiver type
        /// </summary>
        public EmailSendReason SendingReason { get; set; }

        /// <summary>
        /// Email sending reason key
        /// </summary>
        public string SendingReasonKey { get; set; }

        /// <summary>
        /// Resource template name
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Body data model
        /// </summary>
        public object Model { get; set; }

        /// <summary>
        /// From email address
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Create log for email
        /// </summary>
        public bool Log { get; set; }

        /// <summary>
        /// To email addresses
        /// </summary>
        public List<string> To { get; set; }

        /// <summary>
        /// Cc email addresses
        /// </summary>
        public List<string> Cc { get; set; }

        /// <summary>
        /// Bcc email addresses
        /// </summary>
        public List<string> Bcc { get; set; }
    }
}