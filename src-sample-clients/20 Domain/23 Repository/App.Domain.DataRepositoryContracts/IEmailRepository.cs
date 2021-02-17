using App.Domain.Entities.Emails;

namespace App.Domain.DataRepositoryContracts
{
    public interface IEmailRepository : IRepository<EmailLog>
    {
        /// <summary>
        /// Log email sending
        /// </summary>
        /// <param name="emailLog"></param>
        void Log(EmailLog emailLog);
    }
}