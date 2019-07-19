using Signals.Aspects.BackgroundProcessing;
using Signals.Aspects.CommunicationChannels;
using Signals.Aspects.DI;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Configuration.Bootstrapping;
using Signals.Core.Processes.Base;
using Signals.Core.Processes.Distributed;
using Signals.Core.Processes.Recurring;
using Signals.Core.Processes.Recurring.Logging;
using Signals.Core.Processing.Execution;
using Signals.Core.Processing.Results;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Signals.Core.Background.Configuration.Bootstrapping
{
    /// <summary>
    /// Aspects configuration
    /// </summary>
    public class BackgroundApplicationBootstrapConfiguration : ApplicationBootstrapConfiguration
    {
        /// <summary>
        /// Sync logs provider
        /// </summary>
        public new Func<IRecurringTaskLogProvider> RecurringTaskLogProvider { get; set; }

        /// <summary>
        /// Bootstrapping entry
        /// </summary>
        /// <param name="scanAssemblies"></param>
        /// <returns></returns>
        public IServiceContainer Bootstrap(params Assembly[] scanAssemblies)
        {
            if (scanAssemblies == null)
            {
                StackTrace stackTrace = new StackTrace();
                // TODO: workaround for tests
                var assembly = stackTrace.GetFrame(1).GetMethod().DeclaringType.Assembly;

                scanAssemblies = new Assembly[] { assembly };
            }
            return Resolve(scanAssemblies);
        }

        /// <summary>
        /// Build instances from configurations by convention
        /// </summary>
        /// <returns></returns>
        protected override IServiceContainer Resolve(params Assembly[] scanAssemblies)
        {
            var result = base.Resolve(scanAssemblies);
            Start();

            return result;
        }

        /// <summary>
        /// Start background and recurring tasks
        /// </summary>
        private void Start()
        {
            RegisterBackground();
            ScheduleRecurring();
        }

        /// <summary>
        /// Register background tasks
        /// </summary>
        private void RegisterBackground()
        {
            var channel = SystemBootstrapper.GetInstance<IMessageChannel>();

            if (!channel.IsNull())
            {
                var processRepo = SystemBootstrapper.GetInstance<ProcessRepository>();
                processRepo.OfType<IDistributedProcess>().ForEach(type =>
                {
                    channel.Subscribe<string>(type.Name, message =>
                    {
                        var meta = message.Deserialize<DistributedProcessMetadata>();

                        var executor = SystemBootstrapper.GetInstance<IProcessExecutor>();
                        var instance = SystemBootstrapper.GetInstance(type) as IBaseProcess<VoidResult>;
                        SystemBootstrapper.Bootstrap(instance);

                        instance.EpicId = meta.EpicId;

                        executor.ExecuteBackground(instance, meta.Payload);
                    });
                });
            }
        }

        /// <summary>
        /// Schedule recurring tasks
        /// </summary>
        private void ScheduleRecurring()
        {
            var bgRegistry = SystemBootstrapper.GetInstance<ITaskRegistry>();

            if (!bgRegistry.IsNull())
            {
                var processRepo = SystemBootstrapper.GetInstance<ProcessRepository>();
                processRepo.OfType<IRecurringProcess>().ForEach(type =>
                {
                    var instance = SystemBootstrapper.GetInstance(type) as IRecurringProcess;
                    var task = new RecurringTaskWrapper(type);

                    bgRegistry.ScheduleTask(task, instance.Profile);
                });

                bgRegistry.Start();
            }
        }
    }
}