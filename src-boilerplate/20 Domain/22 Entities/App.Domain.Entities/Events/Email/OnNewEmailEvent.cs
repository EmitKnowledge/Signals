using App.Domain.Entities.Events.Base;

namespace App.Domain.Entities.Events.Email
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