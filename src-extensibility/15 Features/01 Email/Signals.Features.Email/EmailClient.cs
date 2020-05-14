using Signals.Core.Common.Smtp;
using Signals.Features.Email.Configurations;
using System.Net;
using System.Net.Mail;

namespace Signals.Features.Email
{
    public class EmailClient : IEmailClient
    {
        private readonly EmailFeatureConfiguration _emailConfiguration;
        private readonly SmtpClientWrapper _smtpClient;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="emailConfiguration"></param>
        public EmailClient(EmailFeatureConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;

            _smtpClient = new SmtpClientWrapper(new SmtpClient
            {
                Host = _emailConfiguration.SmtpConfiguration.Server,
                Port = _emailConfiguration.SmtpConfiguration.Port,
                EnableSsl = _emailConfiguration.SmtpConfiguration.UseSsl,
                Credentials = new NetworkCredential(_emailConfiguration.SmtpConfiguration.Username, _emailConfiguration.SmtpConfiguration.Password)
            });

            _smtpClient.WhitelistedEmails = _emailConfiguration.SmtpConfiguration.WhitelistedEmails;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="mailMessage"></param>
        public void Send(MailMessage mailMessage)
        {
        }

        /// <summary>
        /// Schedule email with key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mailMessage"></param>
        public void Schedule(string key, MailMessage mailMessage)
        {
        }

        /// <summary>
        /// Unschedule email with key
        /// </summary>
        /// <param name="key"></param>
        public void Unschedule(string key)
        {
        }

        /// <summary>
        /// Process all scheaduled messages
        /// </summary>
        public void ProcessScheduledMessages()
        {
        }
    }
}
