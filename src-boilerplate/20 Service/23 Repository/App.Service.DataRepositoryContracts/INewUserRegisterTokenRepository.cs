using App.Service.DomainEntities.Events.Users;

namespace App.Service.DataRepositoryContracts
{
    /// <summary>
    /// Repository for generated confirmation tokens for new user
    /// </summary>
    public interface INewUserRegisterTokenRepository : ITokenBasedEventsRepository<OnNewUserRegisterEvent> { }
}
