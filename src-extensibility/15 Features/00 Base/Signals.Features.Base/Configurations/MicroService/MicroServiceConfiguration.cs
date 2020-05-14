using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Signals.Features.Hosting")]
namespace Signals.Features.Base.Configurations.MicroService
{
    /// <summary>
    /// Feature as micro service configuration
    /// </summary>
    public class MicroServiceConfiguration
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public MicroServiceConfiguration()
        {
            SendRequestsToMicroService = true;
        }

        /// <summary>
        /// Api key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Api secret
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// Host base url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Is current feature client or server
        /// </summary>
        internal bool SendRequestsToMicroService { get; set; }
    }
}
