namespace Signals.Tests.Configuration;

public class StorageConfiguration
{
    public string AccountName { get; set;}
    public string AccountKey { get; set;}
    public string InputDirectoryPath { get; set; }
    public string InputFileName { get; set; }
    public string OutputDirectoryPath { get; set; }
    public string OutputFileName { get; set; }

    public string AzureConnectionString =>
        $"DefaultEndpointsProtocol=https;AccountName=[AccountName];AccountKey=[AccountKey];EndpointSuffix=core.windows.net";
}