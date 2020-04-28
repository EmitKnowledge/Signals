using Signals.Features.Base.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Features.Email.Configurations
{
    public class EmailFeatureConfiguration : IFeatureConfiguration
    {
        public EmailFeatureConfiguration()
        {
            WhitelistedEmails = new List<string>();
        }

        public string Server { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool UseSsl { get; set; }

        public string ConnectionString { get; set; }

        public List<string> WhitelistedEmails { get; set; }

        public MicroServiceConfiguration MicroServiceConfiguration { get; set; }
    }
}
