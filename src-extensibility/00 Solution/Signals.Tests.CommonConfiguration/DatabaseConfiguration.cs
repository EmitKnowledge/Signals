namespace Signals.Tests.Configuration;

public class DatabaseConfiguration
{
    public string Server {get; set; }
    public string Database {get; set;}
    public string UserName {get; set;}
    public string Password {get; set;}
    public string BrokerTable {get; set; }
    
    public string ConnectionString => $"Server={Server};Database={Database};User Id={UserName};Password={Password};";
    
}