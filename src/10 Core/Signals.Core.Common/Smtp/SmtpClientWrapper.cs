using Signals.Core.Common.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Signals.Core.Common.Smtp
{
    /// <summary>
    /// SMTP client wrapper
    /// </summary>
    public class SmtpClientWrapper : SmtpClient, ISmtpClient
    {
        private SmtpClient SmtpClient { get; }

        public List<string> WhitelistedEmails { get; set; }

        public List<string> WhitelistedEmailDomains { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public SmtpClientWrapper(SmtpClient smtpClient)
        {
            SmtpClient = smtpClient;
        }

        /// <summary>
        /// Returns list of whitelisted recipients
        /// </summary>
        /// <param name="recipientsBundle"></param>
        /// <returns></returns>
        private List<string> GetWhitelistedRecipients(List<string> recipients)
        {
            var validRecipients = new List<string>();

            if (recipients.IsNullOrHasZeroElements())
                return validRecipients;

            var whitelistedDomains = WhitelistedEmailDomains?.Select(x => x.ToLower()).ToList();
            var whitelistedEmails = WhitelistedEmails?.Select(x => x.ToLower()).ToList();

            foreach (var recipient in recipients)
            {
                if (recipient.Contains("@"))
                {
                    if (whitelistedDomains.IsNullOrHasZeroElements() && whitelistedEmails.IsNullOrHasZeroElements())
                    {
                        validRecipients.Add(recipient);
                    }
                    else
                    {
                        if (!whitelistedDomains.IsNullOrHasZeroElements())
                        {
                            if (whitelistedDomains.Contains(recipient.Split('@')[1].ToLower()))
                            {
                                validRecipients.Add(recipient);
                            }
                        }
                        else if (!whitelistedEmails.IsNullOrHasZeroElements())
                        {
                            if (whitelistedEmails.Contains(recipient.ToLower()))
                            {
                                validRecipients.Add(recipient);
                            }
                        }
                    }
                }
            }

            return validRecipients;
        }

        /// <summary>
        /// Returns list of whitelisted recipients
        /// </summary>
        /// <param name="recipientsBundle"></param>
        /// <returns></returns>
        private List<string> GetWhitelistedRecipients(MailAddressCollection recipientsBundle)
        {
            var recipients = recipientsBundle.Select(x => x.Address).ToList();
            return GetWhitelistedRecipients(recipients);
        }

        /// <summary>
        /// Returns list of whitelisted recipients
        /// </summary>
        /// <param name="recipientsBundle"></param>
        /// <returns></returns>
        private List<string> GetWhitelistedRecipients(string recipientsBundle)
        {
            var recipients = recipientsBundle?.Split(',', ';').ToList();
            return GetWhitelistedRecipients(recipients);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="from"></param>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public new void Send(string from, string recipients, string subject, string body)
        {
            recipients = string.Join(";", GetWhitelistedRecipients(recipients));
            if (!recipients.IsNullOrEmpty())
            {
                SmtpClient.Send(from, recipients, subject, body);
            }
        }

        /// <summary>
        /// Send async message
        /// </summary>
        /// <param name="from"></param>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="userToken"></param>
        public new async void SendAsync(string from, string recipients, string subject, string body, object userToken)
        {
            recipients = string.Join(";", GetWhitelistedRecipients(recipients));
            if (!recipients.IsNullOrEmpty())
            {
                SmtpClient.SendAsync(from, recipients, subject, body, userToken);
            }
        }

        /// <summary>
        /// Send async message
        /// </summary>
        /// <param name="from"></param>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public new async Task SendMailAsync(string from, string recipients, string subject, string body)
        {
            recipients = string.Join(";", GetWhitelistedRecipients(recipients));
            if (!recipients.IsNullOrEmpty())
            {
                await SmtpClient.SendMailAsync(from, recipients, subject, body);
            }
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message"></param>
        public new void Send(MailMessage message)
        {
            var to = GetWhitelistedRecipients(message.To);
            var cc = GetWhitelistedRecipients(message.CC);
            var bcc = GetWhitelistedRecipients(message.Bcc);

            var originalTo = message.To.ToList();
            var originalCc = message.CC.ToList();
            var originalBcc = message.Bcc.ToList();

            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            to?.ForEach(message.To.Add);
            cc?.ForEach(message.CC.Add);
            bcc?.ForEach(message.Bcc.Add);

            if (!message.To.IsNullOrHasZeroElements())
            {
                SmtpClient.Send(message);
            }

            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            originalTo.ForEach(message.To.Add);
            originalCc.ForEach(message.CC.Add);
            originalBcc.ForEach(message.Bcc.Add);
        }

        /// <summary>
        /// Send async message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userToken"></param>
        public new async void SendAsync(MailMessage message, object userToken)
        {
            var to = GetWhitelistedRecipients(message.To);
            var cc = GetWhitelistedRecipients(message.CC);
            var bcc = GetWhitelistedRecipients(message.Bcc);

            var originalTo = message.To.ToList();
            var originalCc = message.CC.ToList();
            var originalBcc = message.Bcc.ToList();

            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            to?.ForEach(message.To.Add);
            cc?.ForEach(message.CC.Add);
            bcc?.ForEach(message.Bcc.Add);

            if (!message.To.IsNullOrHasZeroElements())
            {
                SmtpClient.SendAsync(message, userToken);
            }

            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            originalTo.ForEach(message.To.Add);
            originalCc.ForEach(message.CC.Add);
            originalBcc.ForEach(message.Bcc.Add);
        }

        /// <summary>
        /// Send async message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public new async Task SendMailAsync(MailMessage message)
        {
            var to = GetWhitelistedRecipients(message.To);
            var cc = GetWhitelistedRecipients(message.CC);
            var bcc = GetWhitelistedRecipients(message.Bcc);

            var originalTo = message.To.ToList();
            var originalCc = message.CC.ToList();
            var originalBcc = message.Bcc.ToList();

            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            to?.ForEach(message.To.Add);
            cc?.ForEach(message.CC.Add);
            bcc?.ForEach(message.Bcc.Add);

            if (!message.To.IsNullOrHasZeroElements())
            {
                await SmtpClient.SendMailAsync(message);
            }

            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            originalTo.ForEach(message.To.Add);
            originalCc.ForEach(message.CC.Add);
            originalBcc.ForEach(message.Bcc.Add);
        }
    }
}
