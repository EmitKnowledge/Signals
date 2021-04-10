using Signals.Aspects.DI;
using Signals.Core.Configuration.Bootstrapping;
using Signals.Core.Processes.Recurring.Logging;
using System.Reflection;

namespace Signals.Core.Background.Configuration.Bootstrapping
{
    /// <summary>
    /// Aspects configuration
    /// </summary>
    public class FluentBackgroundApplicationBootstrapConfiguration : FluentApplicationBootstrapConfiguration, IBackgroundApplicationBootstrapConfiguration
    {
        /// <summary>
        /// Sync logs provider
        /// </summary>
        public IRecurringTaskLogProvider RecurringTaskLogProvider { get; set; }

        /// <summary>
        /// Sync logs provider
        /// </summary>
        public FluentBackgroundApplicationBootstrapConfiguration AddRecurringTaskLogProvider(IRecurringTaskLogProvider recurringTaskLogProvider)
        {
            RecurringTaskLogProvider = recurringTaskLogProvider;
            return this;
        }

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