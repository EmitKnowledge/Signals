using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Features.Base.Configurations
{
    public class MicroServiceConfiguration
    {
        public MicroServiceConfiguration()
        {
            SendRequestsToMicroService = true;
        }

        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        public string Url { get; set; }

        public bool SendRequestsToMicroService { get; set; }
    }
}
