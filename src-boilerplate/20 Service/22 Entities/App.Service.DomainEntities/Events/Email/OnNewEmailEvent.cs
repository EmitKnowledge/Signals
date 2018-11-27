using App.Service.DomainEntities.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Service.DomainEntities.Events.Email
{
    public class OnNewEmailEvent : BaseSystemEvent
    {
        public string Template { get; set; }
        public string SendTo { get; set; }
        public string EmailSenderKey { get; set; }
        public BaseEmailModel Model { get; set; }
        public string ModelFullName { get; set; }
    }
}
