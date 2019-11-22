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
    public interface ISmtpClient
    {
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="from"></param>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        void Send(string from, string recipients, string subject, string body);

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message"></param>
        void Send(MailMessage message);

        /// <summary>
        /// Send async message
        /// </summary>
        /// <param name="from"></param>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="userToken"></param>
        void SendAsync(string from, string recipients, string subject, string body, object userToken);

        /// <summary>
        /// Send async message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userToken"></param>
        void SendAsync(MailMessage message, object userToken);

        /// <summary>
        /// Send async message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMailAsync(MailMessage message);

        /// <summary>
        /// Send async message
        /// </summary>
        /// <param name="from"></param>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task SendMailAsync(string from, string recipients, string subject, string body);
    }
}
