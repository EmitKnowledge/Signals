using Signals.Core.Common.Instance;
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
        private string GetWhitelistedRecipients(string recipientsBundle)
        {
            var recipients = recipientsBundle?.Split(',', ';').ToList();
            var validRecipients = new List<string>();

            var whitelistedDomains = WhitelistedEmailDomains?.Select(x => x.ToLower()).ToList();
            var whitelistedEmails = WhitelistedEmails?.Select(x => x.ToLower()).ToList();

            if (recipients?.Any() == true)
            {
                foreach (var recipient in recipients)
                {
                    if (!recipient.Contains("@"))
                    {
                        continue;
                    }

                    if (whitelistedDomains.IsNullOrHasZeroElements() &&
                        whitelistedEmails.IsNullOrHasZeroElements())
                    {
                        validRecipients.Add(recipient);
                        continue;
                    }

                    if (whitelistedDomains?.Any() == true)
                    {
                        if (whitelistedDomains.Contains(recipient.Split('@')[1].ToLower()))
                        {
                            validRecipients.Add(recipient);
                            continue;
                        }
                    }

                    if (whitelistedEmails?.Any() == true)
                    {
                        if (whitelistedEmails.Contains(recipient.ToLower()))
                        {
                            validRecipients.Add(recipient);
                        }
                    }
                }

                if (validRecipients.Any())
                {
                    return string.Join(";", validRecipients);
                }
            }

            return string.Empty;
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
            recipients = GetWhitelistedRecipients(recipients);
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
            recipients = GetWhitelistedRecipients(recipients);
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
            recipients = GetWhitelistedRecipients(recipients);
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
            var recipients = GetWhitelistedRecipients(string.Join(";", message.To.Select(x => x.Address).ToList()));
            var cc = GetWhitelistedRecipients(string.Join(";", message.CC.Select(x => x.Address).ToList()));
            var bcc = GetWhitelistedRecipients(string.Join(";", message.Bcc.Select(x => x.Address).ToList()));

            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            if (!string.IsNullOrEmpty(recipients))
            {
                recipients.Split(';').ToList().ForEach(x => message.To.Add(x));
            }

            if (!string.IsNullOrEmpty(cc))
            {
                cc.Split(';').ToList().ForEach(x => message.CC.Add(x));
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                bcc.Split(';').ToList().ForEach(x => message.Bcc.Add(x));
            }

            if (!message.To.IsNullOrHasZeroElements())
            {
                SmtpClient.Send(message);
            }
        }

        /// <summary>
        /// Send async message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userToken"></param>
        public new async void SendAsync(MailMessage message, object userToken)
        {
            var recipients = GetWhitelistedRecipients(string.Join(";", message.To.Select(x => x.Address).ToList()));
            var cc = GetWhitelistedRecipients(string.Join(";", message.CC.Select(x => x.Address).ToList()));
            var bcc = GetWhitelistedRecipients(string.Join(";", message.Bcc.Select(x => x.Address).ToList()));

            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            if (!string.IsNullOrEmpty(recipients))
            {
                recipients.Split(';').ToList().ForEach(x => message.To.Add(x));
            }

            if (!string.IsNullOrEmpty(cc))
            {
                cc.Split(';').ToList().ForEach(x => message.CC.Add(x));
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                bcc.Split(';').ToList().ForEach(x => message.Bcc.Add(x));
            }

            if (!message.To.IsNullOrHasZeroElements())
            {
                SmtpClient.SendAsync(message, userToken);
            }
        }

        /// <summary>
        /// Send async message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public new async Task SendMailAsync(MailMessage message)
        {

            var recipients = GetWhitelistedRecipients(string.Join(";", message.To.Select(x => x.Address).ToList()));
            var cc = GetWhitelistedRecipients(string.Join(";", message.CC.Select(x => x.Address).ToList()));
            var bcc = GetWhitelistedRecipients(string.Join(";", message.Bcc.Select(x => x.Address).ToList()));

            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            if (!string.IsNullOrEmpty(recipients))
            {
                recipients.Split(';').ToList().ForEach(x => message.To.Add(x));
            }

            if (!string.IsNullOrEmpty(cc))
            {
                cc.Split(';').ToList().ForEach(x => message.CC.Add(x));
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                bcc.Split(';').ToList().ForEach(x => message.Bcc.Add(x));
            }

            if (!message.To.IsNullOrHasZeroElements())
            {
                await SmtpClient.SendMailAsync(message);
            }
        }
    }
}
