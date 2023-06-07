using Signals.Core.Common.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using MailKit.Security;
using MimeKit;
using SmtpMailClient = MailKit.Net.Smtp.SmtpClient;

namespace Signals.Core.Common.Smtp
{
    /// <summary>
    /// SMTP client wrapper
    /// </summary>
    public class SmtpClientWrapper : ISmtpClient
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        public List<string> WhitelistedEmails { get; set; }

        public List<string> WhitelistedEmailDomains { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public SmtpClientWrapper()
        {
        }
        
        /// <summary>
        /// Send native email via MailKit
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="isSslEnabled"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static async Task SendNativeAsync(
            MailMessage mail,
            string server,
            int port,
            string username,
            string password
            )
        {

            using (var client = new SmtpMailClient())
            {
                client.ServerCertificateValidationCallback = (sender, certificate, certChainType, errors) => true;
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.ConnectAsync(server, port, SecureSocketOptions.Auto);
                await client.AuthenticateAsync(username, password);

                var mimeMessage = new MimeMessage();

                mimeMessage.From.Add(new MailboxAddress(mail.From.DisplayName, mail.From.Address));
                
                if (!mail.To.IsNullOrHasZeroElements())
                {
                    mimeMessage.To.AddRange(
                        mail.To.Select(x => new MailboxAddress(x.DisplayName, x.Address))
                    );
                }
                
                if (!mail.ReplyToList.IsNullOrHasZeroElements())
                {
                    mimeMessage.ReplyTo.AddRange(mail.ReplyToList.Select(x => new MailboxAddress(x.DisplayName, x.Address)));
                }

                if (!mail.CC.IsNullOrHasZeroElements())
                {
                    mimeMessage.Cc.AddRange(
                        mail.CC.Select(x => new MailboxAddress(x.DisplayName, x.Address))
                    );
                }

                if (!mail.Bcc.IsNullOrHasZeroElements())
                {
                    mimeMessage.Bcc.AddRange(
                        mail.Bcc.Select(x => new MailboxAddress(x.DisplayName, x.Address))
                    );
                }

                var builder = new BodyBuilder();
                builder.TextBody = mail.Body;
                builder.HtmlBody = mail.Body;
                
                if (!mail.Attachments.IsNullOrHasZeroElements())
                {
                    foreach (var attachment in mail.Attachments)
                    {
                        await builder.Attachments.AddAsync(attachment.Name, attachment.ContentStream);
                    }
                }

                mimeMessage.Subject = mail.Subject;
                mimeMessage.Body = builder.ToMessageBody();

                await client.SendAsync(mimeMessage);

                await client.DisconnectAsync(true);
            }

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
                var message = new MailMessage(from, recipients, subject, body);
                Task.Run(async () =>
                {
                    await SendNativeAsync(message, Server, Port, Username, Password);
                }).Wait();
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
               var message = new MailMessage(from, recipients, subject, body);
               await SendNativeAsync(message, Server, Port, Username, Password);
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
                Task.Run(async () =>
                {
                    await SendNativeAsync(message, Server, Port, Username, Password);
                }).Wait();
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
                await SendNativeAsync(message, Server, Port, Username, Password);
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
