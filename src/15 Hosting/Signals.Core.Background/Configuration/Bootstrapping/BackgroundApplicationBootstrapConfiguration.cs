using Signals.Aspects.BackgroundProcessing;
using Signals.Aspects.CommunicationChannels;
using Signals.Aspects.DI;
using Signals.Core.Common.Exceptions;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Common.Smtp;
using Signals.Core.Configuration;
using Signals.Core.Configuration.Bootstrapping;
using Signals.Core.Processes.Base;
using Signals.Core.Processes.Distributed;
using Signals.Core.Processes.Recurring;
using Signals.Core.Processes.Recurring.Logging;
using Signals.Core.Processing.Execution;
using Signals.Core.Processing.Results;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.Core.Background.Configuration.Bootstrapping
{
    internal interface IBackgroundApplicationBootstrapConfiguration : IApplicationBootstrapConfiguration
    {
        /// <summary>
        /// Sync logs provider
        /// </summary>
        IRecurringTaskLogProvider RecurringTaskLogProvider { get; set; }
    }

    internal static class BackgroundApplicationBootstrapConfigurationExtensions
    {
        /// <summary>
        /// Bootstrapping entry
        /// </summary>
        /// <param name="backgroundBootstrapConfiguration"></param>
        /// <param name="scanAssemblies"></param>
        /// <returns></returns>
        public static IServiceContainer BootstrapHelper(this IBackgroundApplicationBootstrapConfiguration backgroundBootstrapConfiguration, params Assembly[] scanAssemblies)
        {
            if (scanAssemblies == null)
            {
                StackTrace stackTrace = new StackTrace();
                // TODO: workaround for tests
                var assembly = stackTrace.GetFrame(1).GetMethod().DeclaringType.Assembly;
                scanAssemblies = new Assembly[] { assembly };
                backgroundBootstrapConfiguration.D("Scanning assemblies competed.");
            }

            ConfigurationBootstrapper configurationBootstrapper = new ConfigurationBootstrapper();
            configurationBootstrapper.RecurringTaskLogProvider = () => backgroundBootstrapConfiguration.RecurringTaskLogProvider;
            backgroundBootstrapConfiguration.D("Set Recurring Task Log Provider.");

            var result = backgroundBootstrapConfiguration.Resolve(configurationBootstrapper, scanAssemblies);

            // Proc config validation
            BackgroundApplicationConfiguration config = null;
            try
            {
	            config = BackgroundApplicationConfiguration.Instance;
            }
            catch(Exception ex)
            {
				var dump = ExceptionsExtensions.Extract(
					ex,
					ExceptionsExtensions.ExceptionDetails.Type,
					ExceptionsExtensions.ExceptionDetails.Message,
					ExceptionsExtensions.ExceptionDetails.Stacktrace);
				backgroundBootstrapConfiguration.D($"Exception has occurred while getting the background application configuration instance. Exception: {dump}.");
            }
            finally
            {
	            if (config.IsNull())
	            {
		            backgroundBootstrapConfiguration.D("Signals.Core.Background.Configuration.BackgroundApplicationConfiguration is not provided. Please use a configuration provider to provide configuration values!");
		            throw new Exception("Signals.Core.Background.Configuration.BackgroundApplicationConfiguration is not provided. Please use a configuration provider to provide configuration values!");
	            }
            }

            RegisterBackground();
            ScheduleRecurring();
            NotifyOnStartup();

            return result;
        }
        /// <summary>
        /// Send email on startup
        /// </summary>
        private static void NotifyOnStartup()
        {
            var config = BackgroundApplicationConfiguration.Instance.StartupNotificationConfiguration;
            var smtpClient = SystemBootstrapper.GetInstance<ISmtpClient>();

            if (config.IsNull())
            {
                typeof(BackgroundApplicationBootstrapConfigurationExtensions).D("Startup Notification Configuration not provided. Exit Notify on startup.");
	            return;
            }

            if (smtpClient.IsNull())
            {
	            typeof(BackgroundApplicationBootstrapConfigurationExtensions).D("SMTP client not provided. Exit Notify on startup.");
	            return;
            }

            var tos = config.Emails;
            var subject = config.Subject;
            var body = config.Body;

            if (tos.IsNullOrHasZeroElements()) return;
            if (subject.IsNullOrEmpty()) return;
            if (body.IsNullOrEmpty()) return;

            var from = ApplicationConfiguration.Instance.ApplicationEmail;

            var message = new MailMessage();
            message.From = new MailAddress(@from);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            foreach (var to in tos)
            {
	            message.To.Add(to);
            }

            var sendTask = smtpClient.SendMailAsync(message);
            typeof(BackgroundApplicationBootstrapConfigurationExtensions).D($"Startup Notification email sent to {string.Join(", ", tos)}.");
            Task.WaitAll(sendTask);
        }

        /// <summary>
        /// Register background tasks
        /// </summary>
        private static void RegisterBackground()
        {
            var channel = SystemBootstrapper.GetInstance<IMessageChannel>();
            if (channel.IsNull())
            {
	            typeof(BackgroundApplicationBootstrapConfigurationExtensions).D("Message channel not provided. Exit Register background.");
	            return;
            }

            var processRepo = SystemBootstrapper.GetInstance<ProcessRepository>();
            processRepo.OfType<IDistributedProcess>().ForEach(type =>
            {
	            channel.Subscribe<string>(type.Name, message =>
	            {
		            var meta = message.Deserialize<DistributedProcessMetadata>();

		            var executor = SystemBootstrapper.GetInstance<IProcessExecutor>();
		            var instance = SystemBootstrapper.GetInstance(type) as IBaseProcess<VoidResult>;
		            SystemBootstrapper.Bootstrap(instance);

		            instance.CorrelationId = meta.CorrelationId;
		            instance.CallerProcessName = meta.CallerProcessName;

		            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(meta.CultureName);

		            executor.ExecuteBackground(instance, meta.Payload);
	            });

	            typeof(BackgroundApplicationBootstrapConfigurationExtensions).D($"Message channel subscribed to topic: {type.Name} for process type: {type.FullName}.");

            });
        }

        /// <summary>
        /// Schedule recurring tasks
        /// </summary>
        private static void ScheduleRecurring()
        {
            var bgRegistry = SystemBootstrapper.GetInstance<ITaskRegistry>();
            if (bgRegistry.IsNull())
            {
	            typeof(BackgroundApplicationBootstrapConfigurationExtensions).D("Task Registry not provided. Exit Schedule recurring.");
	            return;
            }

            var processRepo = SystemBootstrapper.GetInstance<ProcessRepository>();
            processRepo.OfType<IRecurringProcess>().ForEach(type =>
            {
	            var instance = SystemBootstrapper.GetInstance(type) as IRecurringProcess;
	            var task = new RecurringTaskWrapper(type);

	            bgRegistry.ScheduleTask(task, instance.Profile);

	            typeof(BackgroundApplicationBootstrapConfigurationExtensions).D($"Recurring task scheduled for process type: {type.FullName}.");
            });

            bgRegistry.Start();
        }
    }

    /// <summary>
    /// Aspects configuration
    /// </summary>
    public class BackgroundApplicationBootstrapConfiguration : ApplicationBootstrapConfiguration, IBackgroundApplicationBootstrapConfiguration
    {
        /// <summary>
        /// Sync logs provider
        /// </summary>
        public IRecurringTaskLogProvider RecurringTaskLogProvider { get; set; }

        /// <summary>
        /// Build instances from configurations by convention
        /// </summary>
        /// <param name="scanAssemblies"></param>
        /// <returns></returns>
        public override IServiceContainer Bootstrap(params Assembly[] scanAssemblies)
        {
            return this.BootstrapHelper(scanAssemblies);
        }
    }
}