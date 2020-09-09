using Signals.Features.Base;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Signals.Features.Email
{
    /// <summary>
    /// Email client contract
    /// </summary>
    public interface IEmailClient : IFeature
    {
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailMessage"></param>
        void Send(EmailMessage emailMessage);

        /// <summary>
        /// Schedule email with key
        /// </summary>
        /// <param name="emailMessage"></param>
        void Schedule(EmailMessage emailMessage);

        /// <summary>
        /// Unschedule email with key
        /// </summary>
        /// <param name="key"></param>
        void Unschedule(string key);

        /// <summary>
        /// Process scheduled messages
        /// </summary>
        void ProcessScheduledMessages();

        /// <summary>
        /// Process all scheaduled messages
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        List<EmailMessage> GetFailedBetweenDates(DateTime start, DateTime end);
    }
}
