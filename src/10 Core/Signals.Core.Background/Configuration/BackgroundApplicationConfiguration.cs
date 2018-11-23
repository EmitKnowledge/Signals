using Signals.Aspects.BackgroundProcessing;
using Signals.Aspects.Bootstrap;
using Signals.Aspects.CommunicationChannels;
using Signals.Aspects.DI;
using Signals.Core.Business.Base;
using Signals.Core.Business.Distributed;
using Signals.Core.Business.Recurring;
using Signals.Core.Common.Instance;
using Signals.Core.Configuration;
using Signals.Core.Processing.Execution;
using Signals.Core.Processing.Results;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Signals.Core.Background.Configuration
{
    /// <summary>
    /// Aspects configuration
    /// </summary>
    public class BackgroundApplicationConfiguration : ApplicationConfiguration
    {
        /// <summary>
        /// Sync logs provider
        /// </summary>
        public Func<ISyncLogProvider> SyncLogProvider { get; set; }

        /// <summary>
        /// Bootstrapping entry
        /// </summary>
        /// <param name="entryAssembly"></param>
        /// <returns></returns>
        public IServiceContainer Bootstrap(params Assembly[] scanAssemblies)
        {
            if (scanAssemblies == null)
            {
                StackTrace stackTrace = new StackTrace();
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
            RegistrationService.Register(SyncLogProvider);

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
                        var executor = SystemBootstrapper.GetInstance<IProcessExecutor>();
                        var instance = SystemBootstrapper.GetInstance(type) as IBaseProcess<VoidResult>;
                        SystemBootstrapper.Bootstrap(instance);
                        executor.ExecuteBackground(instance, message);
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
                    SystemBootstrapper.Bootstrap(instance);

                    var task = new SyncTaskWrapper(type);
                    bgRegistry.ScheduleTask(task, instance.Profile);
                });

                bgRegistry.Start();
            }
        }
    }
}