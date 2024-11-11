using Newtonsoft.Json;
using Signals.Aspects.Configuration;
using Signals.Aspects.Configuration.File;

namespace Signals.Tests.Configuration;

public class BaseTestConfiguration
{
    // Lazy instance of Config, initialized only when accessed
    private static readonly Lazy<BaseTestConfiguration> _instance = new Lazy<BaseTestConfiguration>(() => new BaseTestConfiguration());
    public static BaseTestConfiguration Instance => _instance.Value;
    
    public DatabaseConfiguration DatabaseConfiguration { get; set; }
    public SmtpConfiguration SmtpConfiguration { get; set; }
    public StorageConfiguration StorageConfiguration { get; set; }
    public ServiceBusConfiguration ServiceBusConfiguration { get; set; }
    
    // Define a temporary class to avoid recursion
    private class TempConfig
    {
        public DatabaseConfiguration DatabaseConfiguration { get; set; }
        public SmtpConfiguration SmtpConfiguration { get; set; }
        public StorageConfiguration StorageConfiguration { get; set; }
        public ServiceBusConfiguration ServiceBusConfiguration { get; set; }
    }
    

    public BaseTestConfiguration()
    {
        LoadConfigurationFromFile();
    }
    
    private void LoadConfigurationFromFile()
    {
        string configPath = Path.Combine(AppContext.BaseDirectory, "config\\testConfig.json");
        
        if (File.Exists(configPath))
        {
            var json = File.ReadAllText(configPath);
            var config = JsonConvert.DeserializeObject<TempConfig>(json);
        
            // Map deserialized values to this instance
            DatabaseConfiguration = config.DatabaseConfiguration;
            SmtpConfiguration = config.SmtpConfiguration;
            StorageConfiguration = config.StorageConfiguration;
            ServiceBusConfiguration = config.ServiceBusConfiguration;
        }
        else
        {
            throw new FileNotFoundException("Configuration file not found.");
        }
    }
}

