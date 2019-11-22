using Signals.Core.Common.Instance;
using Signals.Core.Configuration;
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
        private SmtpClient _smtpClient { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        public SmtpClientWrapper(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
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
            if (ApplicationConfiguration.Instance.WhitelistedEmails.IsNullOrHasZeroElements())
            {
                _smtpClient.Send(from, recipients, subject, body);
            }
            else
            {
                var to = recipients?.Split(',', ';');
                if (!to.IsNullOrHasZeroElements())
                {
                    var whitelistedTo = ApplicationConfiguration.Instance.WhitelistedEmails;
                    var verifiedTo = to.Where(x => whitelistedTo.Contains(x)).ToList();

                    if (!verifiedTo.IsNullOrHasZeroElements())
                        _smtpClient.Send(from, string.Join(";", verifiedTo), subject, body);
                }
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
            if (ApplicationConfiguration.Instance.WhitelistedEmails.IsNullOrHasZeroElements())
            {
                _smtpClient.SendAsync(from, recipients, subject, body, userToken);
            }
            else
            {
                var to = recipients?.Split(',', ';');
                if (!to.IsNullOrHasZeroElements())
                {
                    var whitelistedTo = ApplicationConfiguration.Instance.WhitelistedEmails;
                    var verifiedTo = to.Where(x => whitelistedTo.Contains(x)).ToList();

                    if (!verifiedTo.IsNullOrHasZeroElements())
                        _smtpClient.SendAsync(from, string.Join(";", verifiedTo), subject, body, userToken);
                }
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
            if (ApplicationConfiguration.Instance.WhitelistedEmails.IsNullOrHasZeroElements())
            {
                await _smtpClient.SendMailAsync(from, recipients, subject, body);
            }
            else
            {
                var to = recipients?.Split(',', ';');
                if (!to.IsNullOrHasZeroElements())
                {
                    var whitelistedTo = ApplicationConfiguration.Instance.WhitelistedEmails;
                    var verifiedTo = to.Where(x => whitelistedTo.Contains(x)).ToList();

                    if (!verifiedTo.IsNullOrHasZeroElements())
                        await _smtpClient.SendMailAsync(from, string.Join(";", verifiedTo), subject, body);
                }
            }
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message"></param>
        public new void Send(MailMessage message)
        {
            if (ApplicationConfiguration.Instance.WhitelistedEmails.IsNullOrHasZeroElements())
            {
                _smtpClient.Send(message);
            }
            else
            {
                if (!message.To.IsNullOrHasZeroElements())
                {
                    var whitelistedTo = ApplicationConfiguration.Instance.WhitelistedEmails;

                    var verifiedTo = message.To.Where(x => whitelistedTo.Contains(x.Address)).ToList();
                    message.To.Clear();
                    verifiedTo.ForEach(x => message.To.Add(x));

                    var verifiedCc = message.CC.Where(x => whitelistedTo.Contains(x.Address)).ToList();
                    message.CC.Clear();
                    verifiedCc.ForEach(x => message.CC.Add(x));

                    var verifiedBcc = message.Bcc.Where(x => whitelistedTo.Contains(x.Address)).ToList();
                    message.Bcc.Clear();
                    verifiedBcc.ForEach(x => message.Bcc.Add(x));

                    if (!verifiedTo.IsNullOrHasZeroElements())
                        _smtpClient.Send(message);
                }
            }
        }

        /// <summary>
        /// Send async message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userToken"></param>
        public new async void SendAsync(MailMessage message, object userToken)
        {
            if (ApplicationConfiguration.Instance.WhitelistedEmails.IsNullOrHasZeroElements())
            {
                _smtpClient.SendAsync(message, userToken);
            }
            else
            {
                if (!message.To.IsNullOrHasZeroElements())
                {
                    var whitelistedTo = ApplicationConfiguration.Instance.WhitelistedEmails;

                    var verifiedTo = message.To.Where(x => whitelistedTo.Contains(x.Address)).ToList();
                    message.To.Clear();
                    verifiedTo.ForEach(x => message.To.Add(x));

                    var verifiedCc = message.CC.Where(x => whitelistedTo.Contains(x.Address)).ToList();
                    message.CC.Clear();
                    verifiedCc.ForEach(x => message.CC.Add(x));

                    var verifiedBcc = message.Bcc.Where(x => whitelistedTo.Contains(x.Address)).ToList();
                    message.Bcc.Clear();
                    verifiedBcc.ForEach(x => message.Bcc.Add(x));

                    if (!verifiedTo.IsNullOrHasZeroElements())
                        _smtpClient.SendAsync(message, userToken);
                }
            }
        }

        /// <summary>
        /// Send async message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public new async Task SendMailAsync(MailMessage message)
        {
            if (ApplicationConfiguration.Instance.WhitelistedEmails.IsNullOrHasZeroElements())
            {
                await _smtpClient.SendMailAsync(message);
            }
            else
            {
                if (!message.To.IsNullOrHasZeroElements())
                {
                    var whitelistedTo = ApplicationConfiguration.Instance.WhitelistedEmails;

                    var verifiedTo = message.To.Where(x => whitelistedTo.Contains(x.Address)).ToList();
                    message.To.Clear();
                    verifiedTo.ForEach(x => message.To.Add(x));

                    var verifiedCc = message.CC.Where(x => whitelistedTo.Contains(x.Address)).ToList();
                    message.CC.Clear();
                    verifiedCc.ForEach(x => message.CC.Add(x));

                    var verifiedBcc = message.Bcc.Where(x => whitelistedTo.Contains(x.Address)).ToList();
                    message.Bcc.Clear();
                    verifiedBcc.ForEach(x => message.Bcc.Add(x));

                    if (!verifiedTo.IsNullOrHasZeroElements())
                        await _smtpClient.SendMailAsync(message);
                }
            }
        }
    }
}
