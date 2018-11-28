using App.Domain.DataRepository.Base;
using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Events.Users;
using Signals.Aspects.DI.Attributes;

namespace App.Domain.DataRepository.Tokens
{
    /// <summary>
    /// Repository for generated confirmation tokens for new user
    /// </summary>
    [Export(typeof(INewUserRegisterTokenRepository))]
    internal class NewUserRegisterTokenRepository : BaseTokenRepository<OnNewUserRegisterEvent>, INewUserRegisterTokenRepository { }
}