using Signals.Core.Common.Smtp;
using Signals.Features.Email.Configurations;
using Signals.Features.Email.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Signals.Features.Email
{
    /// <summary>
    /// Email feature client
    /// </summary>
    public class EmailClient : IEmailClient
    {
        private readonly Repository _repository;
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
            _repository = new Repository(_emailConfiguration.ConnectionString, _emailConfiguration.TableName);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailMessage"></param>
        public void Send(EmailMessage emailMessage)
        {
            try
            {
                emailMessage.Validate();
                _smtpClient.Send(emailMessage);
                emailMessage.IsSent = true;
            }
            catch (Exception ex)
            {
                emailMessage.Error = ex.Message;
            }
            finally
            {
                _repository.Insert(emailMessage);
            }
        }

        /// <summary>
        /// Schedule email with key
        /// </summary>
        /// <param name="emailMessage"></param>
        public void Schedule(EmailMessage emailMessage)
        {
            try
            {
                emailMessage.Validate(true);
                emailMessage.IsSent = false;
            }
            catch (Exception ex)
            {
                emailMessage.Error = ex.Message;
            }
            finally
            {
                _repository.Insert(emailMessage);
            }
        }

        /// <summary>
        /// Unschedule email with key
        /// </summary>
        /// <param name="key"></param>
        public void Unschedule(string key)
        {
            _repository.Unschedule(key);
        }

        /// <summary>
        /// Process all scheaduled messages
        /// </summary>
        public void ProcessScheduledMessages()
        {
            var emailMessages = _repository.GetDueEmails();

            foreach (var emailMessage in emailMessages)
            {

                try
                {
                    _smtpClient.Send(emailMessage);
                    _repository.MarkAsSent(emailMessage.Id);
                }
                catch (Exception ex)
                {
                    _repository.MarkAsError(emailMessage.Id, ex.Message);
                }
            }
        }

        /// <summary>
        /// Process all scheaduled messages
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<EmailMessage> GetFailedBetweenDates(DateTime start, DateTime end)
        {
            return _repository.GetFailedBetweenDates(start, end);
        }
    }
}
