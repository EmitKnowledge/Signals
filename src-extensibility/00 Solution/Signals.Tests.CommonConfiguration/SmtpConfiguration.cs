namespace Signals.Tests.Configuration;

public class SmtpConfiguration
{
    public string Server { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string RootEmail { get; set; }
    public string RootDomain { get; set; }
    
}