namespace Signals.Tests.Configuration.CustomConfigurations.MsSql.Controllers
{
    public class SecurityConfiguration
    {
        public int SaltLength { get; set; }

        public int MaxPasswordLength { get; set; }

        public int MinPasswordLength { get; set; }

        public int TokenLength { get; set; }

    }
}
