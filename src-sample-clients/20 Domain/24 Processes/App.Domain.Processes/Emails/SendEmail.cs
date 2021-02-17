using App.Common.Helpers.Localizaiton;
using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Emails;
using App.Domain.Processes.Emails.Dtos.Request;
using App.Domain.Processes.Generic.Specification;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Smtp;
using Signals.Core.Configuration;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;
using System;
using System.Net;
using System.Net.Mail;

namespace App.Domain.Processes.Emails
{
    public partial class EmailProcesses
    {
        public class SendEmail : BusinessProcess<SendEmailRequestDto, VoidResult>
        {
            private static object @lock = new object();
            [Import] private ISmtpClient SmtpClient { get; set; }
            [Import] private IEmailRepository EmailRepository { get; set; }

            /// <summary>
            /// Auth procecss
            /// </summary>
            /// <param name="dto"></param>
            /// <returns></returns>
            public override VoidResult Auth(SendEmailRequestDto dto)
            {
                return Ok();
            }

            /// <summary>
            /// Validate procecss
            /// </summary>
            /// <param name="dto"></param>
            /// <returns></returns>
            public override VoidResult Validate(SendEmailRequestDto dto)
            {
                return BeginValidation()
                    .Validate(new NotNullEntity<SendEmailRequestDto>(), dto)
                    .Validate(new NotNullOrEmptyString(), dto.Template)
                    .Validate(new NotNullOrEmptyStrings(), dto.To)
                    .ReturnResult();
            }

            /// <summary>
            /// Handle procecss
            /// </summary>
            /// <param name="dto"></param>
            /// <returns></returns>
            public override VoidResult Handle(SendEmailRequestDto dto)
            {
                var from = dto.From ?? ApplicationConfiguration.Instance.ApplicationEmail;

                var message = new MailMessage();

                message.From = new MailAddress(from);

                dto.To.ForEach(message.To.Add);
                dto.Cc.ForEach(message.CC.Add);
                dto.Bcc.ForEach(message.Bcc.Add);

                message.IsBodyHtml = true;

                var subjectTemplate = Context.LocalizationProvider.Get("SUBJECT", dto.Template, "mailmessages");
                var bodyTemplate = Context.LocalizationProvider.Get("BODY", dto.Template, "mailmessages");

                if (dto.Model.IsNull())
                {
                    message.Subject = subjectTemplate.Value;
                    message.Body = bodyTemplate.Value;
                }
                else
                {
                    message.Subject = subjectTemplate.RenderTemplate(dto.Model);
                    message.Body = bodyTemplate.RenderTemplate(dto.Model);
                }

                var log = new EmailLog();
                log.Success = true;
                Exception exception = null;

                try
                {
                    lock (@lock)
                    {
                        var oldSecurityProtocol = ServicePointManager.SecurityProtocol;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        SmtpClient.Send(message);
                        ServicePointManager.SecurityProtocol = oldSecurityProtocol;
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                    if (dto.Log)
                    {
                        log.Exception = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}";
                        log.Success = false;
                    }
                    else
                    {
                        // throw;
                        return Fail(exception);
                    }
                }

                if (dto.Log)
                {
                    log.From = from;
                    log.To = string.Join(" | ", dto.To);
                    log.Cc = string.Join(" | ", dto.Cc);
                    log.Bcc = string.Join(" | ", dto.Bcc);
                    log.Body = message.Body;
                    log.Subject = message.Subject;
                    log.SendingReasonKey = dto.SendingReasonKey;
                    log.SendingReason = dto.SendingReason.ToString();

                    EmailRepository.Log(log);
                }

                if (!exception.IsNull())
                    return Fail(exception);

                return Ok();
            }
        }
    }
}