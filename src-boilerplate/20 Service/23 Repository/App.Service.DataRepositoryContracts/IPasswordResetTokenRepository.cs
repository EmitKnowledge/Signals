using App.Service.DomainEntities.Events.Users;

namespace App.Service.DataRepositoryContracts
{
    /// <summary>
    /// Repository for generated confirmation tokens for pasword reset actions
    /// </summary>
    public interface IPasswordResetTokenRepository : ITokenBasedEventsRepository<OnPasswordResetEvent> { }
}