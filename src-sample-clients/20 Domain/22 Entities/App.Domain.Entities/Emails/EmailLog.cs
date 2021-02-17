using App.Domain.Entities.Base;

namespace App.Domain.Entities.Emails
{
    public class EmailLog : BaseDomainEntity
    {
        /// <summary>
        /// Email from address
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Email to | separated addresses
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Email CC to | separated addresses
        /// </summary>
        public string Cc { get; set; }

        /// <summary>
        /// Email BCC to | separated addresses
        /// </summary>
        public string Bcc { get; set; }

        /// <summary>
        /// Email subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Email body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Email log exception message if sending is unsuccessful
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// Is email sending unsuccessful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Email sent because: reason type
        /// </summary>
        public string SendingReason { get; set; }

        /// <summary>
        /// Email sent because: reason reference key
        /// </summary>
        public string SendingReasonKey { get; set; }
    }
}