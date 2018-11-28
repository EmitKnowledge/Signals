using App.Domain.Entities.Events.Users;

namespace App.Domain.DataRepositoryContracts
{
    /// <summary>
    /// Repository for generated confirmation tokens for new user
    /// </summary>
    public interface INewUserRegisterTokenRepository : ITokenBasedEventsRepository<OnNewUserRegisterEvent> { }
}