using App.Domain.Entities.Events.Users;

namespace App.Domain.DataRepositoryContracts
{
    /// <summary>
    /// Repository for generated confirmation tokens for pasword reset actions
    /// </summary>
    public interface IPasswordResetTokenRepository : ITokenBasedEventsRepository<OnPasswordResetEvent> { }
}