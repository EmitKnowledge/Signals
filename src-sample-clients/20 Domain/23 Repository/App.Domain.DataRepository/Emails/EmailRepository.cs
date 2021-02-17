using App.Domain.DataRepository.Base;
using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Emails;
using Signals.Aspects.DI.Attributes;

namespace App.Domain.DataRepository.Emails
{
    [Export(typeof(IEmailRepository))]
    internal class EmailRepository : BaseDbRepository<EmailLog>, IEmailRepository
    {
        /// <summary>
        /// Log email sending
        /// </summary>
        /// <param name="emailLog"></param>
        public void Log(EmailLog emailLog)
        {
            Insert(emailLog);
        }
    }
}