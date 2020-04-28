using Signals.Core.Common.Smtp;
using Signals.Features.Base;
using Signals.Features.Base.Web;
using Signals.Features.Email.Configurations;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Signals.Features.Email
{
    public class EmailClient : BaseClient, IEmailClient, IFeature
    {
        private readonly EmailFeatureConfiguration _emailConfiguration;
        private readonly SmtpClientWrapper _smtpClient;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="emailConfiguration"></param>
        public EmailClient(EmailFeatureConfiguration emailConfiguration) : base(emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;

            _smtpClient = new SmtpClientWrapper(new SmtpClient
            {
                Host = _emailConfiguration.Server,
                Port = _emailConfiguration.Port,
                EnableSsl = _emailConfiguration.UseSsl,
                Credentials = new NetworkCredential(_emailConfiguration.Username, _emailConfiguration.Password)
            });

            _smtpClient.WhitelistedEmails = _emailConfiguration.WhitelistedEmails;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="mailMessage"></param>
        public void Send(MailMessage mailMessage)
        {
            Process("Send",
                new Dictionary<string, object> {
                    { "mailMessage", mailMessage }
                },
                () =>
                {

                });
        }

        /// <summary>
        /// Schedule email with key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mailMessage"></param>
        public void Schedule(string key, MailMessage mailMessage)
        {
            Process("Schedule",
                new Dictionary<string, object> {
                    { "key", key },
                    { "mailMessage", mailMessage }
                },
                () =>
                {

                });
        }

        /// <summary>
        /// Unschedule email with key
        /// </summary>
        /// <param name="key"></param>
        public void Unschedule(string key)
        {
            Process("Unschedule",
                new Dictionary<string, object> {
                    { "key", key }
                },
                () =>
                {

                });
        }

        public void ProcessScheduledMessages()
        {
            Process("ProcessScheduledMessages",
                new Dictionary<string, object> { },
                () =>
                {

                });
        }
    }
}
