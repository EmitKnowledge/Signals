using App.Domain.DataRepository.Base;
using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Events.Users;
using Signals.Aspects.DI.Attributes;

namespace App.Domain.DataRepository.Tokens
{
    /// <summary>
    /// Repository for generated confirmation tokens for pasword reset actions
    /// </summary>
    [Export(typeof(IPasswordResetTokenRepository))]
    internal class PasswordResetTokenRepository : BaseTokenRepository<OnPasswordResetEvent>, IPasswordResetTokenRepository { }
}