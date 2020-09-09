using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Signals.Features.Email
{
    /// <summary>
    /// Email attachment
    /// </summary>
    public class EmailAttachment
    {
        /// <summary>
        /// Attachment content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Attachment mime type
        /// </summary>
        public string MimeType { get; set; }
    }

    /// <summary>
    /// Email message
    /// </summary>
    public class EmailMessage
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public EmailMessage()
        {
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();
            ReplyTo = new List<string>();
            Attachments = new List<EmailAttachment>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Table row created on
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Email scheduled for date
        /// </summary>
        public DateTime? ScheduledFor { get; set; }

        /// <summary>
        /// Is email message sent
        /// </summary>
        public bool IsSent { get; set; }

        /// <summary>
        /// Email reference key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Email sender
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// From
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Body encoding
        /// </summary>
        public string BodyEncoding { get; set; }

        /// <summary>
        /// Subject encoding
        /// </summary>
        public string SubjectEncoding { get; set; }

        /// <summary>
        /// Is body html message
        /// </summary>
        public bool IsBodyHtml { get; set; }

        /// <summary>
        /// List of attachment files
        /// </summary>
        public List<EmailAttachment> Attachments { get; set; }

        /// <summary>
        /// List of receivers
        /// </summary>
        public List<string> To { get; set; }

        /// <summary>
        /// List of CC receivers
        /// </summary>
        public List<string> Cc { get; set; }

        /// <summary>
        /// List of BCC receivers
        /// </summary>
        public List<string> Bcc { get; set; }

        /// <summary>
        /// List of reply receivers
        /// </summary>
        public List<string> ReplyTo { get; set; }

        /// <summary>
        /// Error occured while sending
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Operator
        /// </summary>
        /// <param name="emailMessage"></param>
        public static implicit operator MailMessage(EmailMessage emailMessage)
        {
            var mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(emailMessage.From);
            mailMessage.Subject = emailMessage.Subject;
            mailMessage.Body = emailMessage.Body;
            mailMessage.IsBodyHtml = emailMessage.IsBodyHtml;

            emailMessage.To.ForEach(mailMessage.To.Add);
            emailMessage.Cc.ForEach(mailMessage.CC.Add);
            emailMessage.Bcc.ForEach(mailMessage.Bcc.Add);
            emailMessage.ReplyTo.ForEach(mailMessage.ReplyToList.Add);
            emailMessage.Attachments.ForEach(attachment => mailMessage.Attachments.Add(Attachment.CreateAttachmentFromString(attachment.Content, attachment.MimeType)));

            if (!string.IsNullOrEmpty(emailMessage.BodyEncoding))
                mailMessage.BodyEncoding = Encoding.GetEncoding(emailMessage.BodyEncoding);

            if (!string.IsNullOrEmpty(emailMessage.SubjectEncoding))
                mailMessage.SubjectEncoding = Encoding.GetEncoding(emailMessage.SubjectEncoding);

            if (!string.IsNullOrEmpty(emailMessage.Sender))
                mailMessage.Sender = new MailAddress(emailMessage.Sender);

            return mailMessage;
        }

        /// <summary>
        /// Validate email message
        /// </summary>
        /// <param name="isSchedule"></param>
        internal void Validate(bool isSchedule = false)
        {
            if (string.IsNullOrEmpty(Subject))
                throw new ArgumentNullException(nameof(Subject));

            if (string.IsNullOrEmpty(Body))
                throw new ArgumentNullException(nameof(Body));

            if (string.IsNullOrEmpty(From))
                throw new ArgumentNullException(nameof(From));

            if (To?.Any() != true)
                throw new ArgumentNullException(nameof(To));

            if (!string.IsNullOrEmpty(SubjectEncoding) && Encoding.GetEncoding(SubjectEncoding) == null)
                throw new ArgumentException($"Argument {nameof(SubjectEncoding)} has unsupported value {SubjectEncoding}");

            if (!string.IsNullOrEmpty(BodyEncoding) && Encoding.GetEncoding(BodyEncoding) == null)
                throw new ArgumentException($"Argument {nameof(BodyEncoding)} has unsupported value {BodyEncoding}");

            if (isSchedule)
            {
                if (string.IsNullOrEmpty(Key))
                    throw new ArgumentNullException(nameof(Key));

                if (!ScheduledFor.HasValue)
                    throw new ArgumentNullException(nameof(ScheduledFor));
            }

            var mailMessage = (MailMessage)this;
        }
    }
}
