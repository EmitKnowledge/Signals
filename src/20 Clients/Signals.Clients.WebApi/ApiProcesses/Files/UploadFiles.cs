using Signals.Core.Processes.Api;

namespace Signals.Clients.WebApi.ApiProcesses.Files
{
    [SignalsApi(HttpMethod = SignalsApiMethod.POST)]
    public class StoreFiles : AutoApiProcess<BusinessProcesses.Files.StoreFiles>
    {
    }
}
