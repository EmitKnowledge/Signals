using Ganss.XSS;
using Signals.Core.Processing.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Signals.Clients.MvcWeb.BusinessProcesses
{
    public class Data : IDtoData
    {
        private static int i = 0;

        public int Number => i++;

        public void Sanitize(HtmlSanitizer sanitizer)
        {
        }
    }

    public class DataResponse : IDtoData
    {
        private static int i = 0;

        public int Number { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
        }
    }

    public class NotifData : ITransientData
    {
        private static int i = 0;

        public int Number => i--;
    }
}
