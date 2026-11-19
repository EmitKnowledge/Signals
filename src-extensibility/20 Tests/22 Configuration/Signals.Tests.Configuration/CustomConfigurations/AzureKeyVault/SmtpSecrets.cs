using System.ComponentModel.DataAnnotations;

namespace Signals.Tests.Configuration.CustomConfigurations.AzureKeyVault
{
    public class SmtpSecrets
    {
        public string Username { get; set; }

        [Required(ErrorMessage = "SMTP password is required.")]
        public string Password { get; set; }
    }
}
