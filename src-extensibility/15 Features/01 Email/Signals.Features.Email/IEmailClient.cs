using Signals.Features.Base;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Signals.Features.Email
{
    public interface IEmailClient : IFeature
    {
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="mailMessage"></param>
        void Send(MailMessage mailMessage);

        /// <summary>
        /// Schedule email with key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mailMessage"></param>
        void Schedule(string key, MailMessage mailMessage);

        /// <summary>
        /// Unschedule email with key
        /// </summary>
        /// <param name="key"></param>
        void Unschedule(string key);

        /// <summary>
        /// Process scheduled messages
        /// </summary>
        void ProcessScheduledMessages();
    }
}
