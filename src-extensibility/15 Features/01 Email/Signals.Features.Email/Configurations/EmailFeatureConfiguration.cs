using Signals.Features.Base.Configurations.Feature;
using Signals.Features.Base.Configurations.MicroService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Features.Email.Configurations
{
    /// <summary>
    /// Email feature configuratiion
    /// </summary>
    public class EmailFeatureConfiguration : IFeatureConfiguration
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public EmailFeatureConfiguration()
        {
            SmtpConfiguration = new SmtpConfigurationElement();
        }
        

        public class SmtpConfigurationElement
        {
            /// <summary>
            /// CTOR
            /// </summary>
            public SmtpConfigurationElement()
            {
                WhitelistedEmails = new List<string>();
            }

            /// <summary>
            /// Server
            /// </summary>
            public string Server { get; set; }

            /// <summary>
            /// Port
            /// </summary>
            public int Port { get; set; }

            /// <summary>
            /// Username
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Password
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// Use https protocol
            /// </summary>
            public bool UseSsl { get; set; }

            /// <summary>
            /// List of whitelisted emails
            /// </summary>
            public List<string> WhitelistedEmails { get; set; }
        }

        public SmtpConfigurationElement SmtpConfiguration { get; set; }

        public string ConnectionString { get; set; }

        public MicroServiceConfiguration MicroServiceConfiguration { get; set; }
    }
}
