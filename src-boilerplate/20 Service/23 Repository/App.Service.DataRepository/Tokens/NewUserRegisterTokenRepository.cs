using App.Service.DataRepository.Base;
using App.Service.DataRepositoryContracts;
using App.Service.DomainEntities.Events.Users;
using Signals.Aspects.DI.Attributes;

namespace App.Service.DataRepository.Tokens
{
    /// <summary>
    /// Repository for generated confirmation tokens for new user
    /// </summary>
    [Export(typeof(INewUserRegisterTokenRepository))]
    internal class NewUserRegisterTokenRepository : BaseTokenRepository<OnNewUserRegisterEvent>, INewUserRegisterTokenRepository { }
}