using Signals.Aspects.Bootstrap;
using Signals.Core.Business.Base;
using Signals.Core.Common.Instance;
using Signals.Core.Configuration;
using Signals.Core.Processing.Behaviour;
using Signals.Core.Processing.Results;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

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
                if (ApplicationConfiguration.Instance?.CriticalConfiguration != null)
                {
                    string InterpolateException(string originalString)
                    {
                        return originalString
                            .Replace("[#Message#]", ex.Message)
                            .Replace("[#Process#]", processType.Name)
                            .Replace("[#Message#]", ex.Message)
                            .Replace("[#Stack#]", ex.StackTrace)
                            .Replace("[#StackTrace#]", ex.StackTrace);
                    }

                    var client = SystemBootstrapper.GetInstance<SmtpClient>();

                    if (!client.IsNull())
                    {
                        var criticalAttributes = processType.GetCustomAttributes(typeof(CriticalAttribute), true).Cast<CriticalAttribute>().ToList();

                        List<Task> sendingEmailTasks = new List<Task>();
                        foreach (var attribute in criticalAttributes)
                        {
                            var email = attribute.NotificaitonEmail;

                            if (!email.IsNull())
                            {
                                var from = ApplicationConfiguration.Instance.ApplicationEmail;
                                var to = email;
                                var subject = InterpolateException(ApplicationConfiguration.Instance.CriticalConfiguration.Subject);
                                var body = InterpolateException(ApplicationConfiguration.Instance.CriticalConfiguration.Body);

                                var stream = new MemoryStream();
                                var writer = new StreamWriter(stream);
                                writer.WriteLine(
								$@"Date :{DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}{Environment.NewLine}
								Message :{ex.Message}{Environment.NewLine}
								StackTrace :{ex.StackTrace}");

                                stream.Position = 0;
                                Attachment attachment = new Attachment(stream, "text/text");

                                var message = new MailMessage(from, to, subject, body);
                                message.Attachments.Add(attachment);

                                var sendTask = client
                                    .SendMailAsync(message)
                                    .ContinueWith(task => writer.Dispose());

                                sendingEmailTasks.Add(sendTask);
                            }
                        }

                        if (sendingEmailTasks.Any())
                            Task.WaitAll(sendingEmailTasks.ToArray());
                    }
                }

                throw;
            }
        }
    }
}
