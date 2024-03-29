﻿using Signals.Aspects.DI;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Common.Smtp;
using Signals.Core.Configuration;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Behaviour;
using Signals.Core.Processing.Results;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Signals.Core.Common.Reflection;

namespace Signals.Core.Processing.Execution.ExecutionHandlers
{
    /// <summary>
    /// Process execution handler
    /// </summary>
    internal class CriticalNotifyingHandler : IExecutionHandler
    {
        /// <summary>
        /// Next handler in execution pipe
        /// </summary>
        public IExecutionHandler Next { get; set; }

        /// <summary>
        /// Execute handler
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="process"></param>
        /// <param name="processType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public TResult Execute<TResult>(IBaseProcess<TResult> process, Type processType, params object[] args) where TResult : VoidResult, new()
        {
            try
            {
                return Next.Execute(process, processType, args);
            }
            catch (Exception ex)
            {
                var criticalAttributes = processType.GetCachedAttributes<CriticalAttribute>();

                if (criticalAttributes.Any())
                {
                    var manager = SystemBootstrapper.GetInstance<CriticalErrorCallbackManager>();
                    manager?.InvokeError(process, processType, args, ex);
                }

                var happeningDate = DateTime.UtcNow;
                if (ApplicationConfiguration.Instance?.CriticalConfiguration == null)
                {
	                this.D($"No critical configuration has been found for process type: {processType?.FullName}. Exit handler.");
	                throw;
                }

                string InterpolateException(string originalString)
                {
	                return originalString
		                .Replace("[#Date#]", happeningDate.ToString(CultureInfo.InvariantCulture))
		                .Replace("[#Process#]", processType.Name)
		                .Replace("[#Message#]", ex.Message)
		                .Replace("[#StackTrace#]", ex.StackTrace)
		                .Replace("[#Data#]", args?.SerializeJson());
                }

                var client = SystemBootstrapper.GetInstance<ISmtpClient>();
                if (client.IsNull())
                {
	                this.D($"No SMTP client has been found for process type: {processType?.FullName}. Exit handler.");
                    throw;
                }

                foreach (var attribute in criticalAttributes)
                {
	                var emails = attribute.NotificaitonEmails?.Split(',')?.Select(x => x.Trim())?.ToList();

	                if (!emails.IsNullOrHasZeroElements())
	                {
		                var from = ApplicationConfiguration.Instance.ApplicationEmail;
		                var subject = InterpolateException(ApplicationConfiguration.Instance.CriticalConfiguration.Subject);
		                var body = InterpolateException(ApplicationConfiguration.Instance.CriticalConfiguration.Body);

		                var data = $@"Date: {happeningDate.ToString(CultureInfo.InvariantCulture)}{Environment.NewLine}
Process: {processType.Name}{Environment.NewLine}
Message: {ex.Message}{Environment.NewLine}
StackTrace: {ex.StackTrace}{Environment.NewLine}
Data: {args?.SerializeJson()}{Environment.NewLine}";

		                Attachment attachment = Attachment.CreateAttachmentFromString(data, "critical_info.txt");

		                var message = new MailMessage();
		                message.From = new MailAddress(@from);
		                message.Subject = subject;
		                message.Body = body;
		                message.IsBodyHtml = true;

		                foreach (var email in emails)
		                {
			                message.To.Add(email);
		                }

		                message.Attachments.Add(attachment);
		                client.Send(message);
		                this.D($"Critical email has been sent to: {string.Join(", ", emails)} for process type: {processType?.Name}. Exception: {ex.Message}");
                    }
                }

                this.D("Executed -> Critical Notifying Handler");
                throw;
            }
        }
    }
}
