namespace Signals.Tests.Configuration;

public class ServiceBusConfiguration
{
    public string Endpoint { get; set; }
    public string AccessKey { get; set; }

    public string ConnectionString =>
        $@"Endpoint={Endpoint};SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={AccessKey}";
}