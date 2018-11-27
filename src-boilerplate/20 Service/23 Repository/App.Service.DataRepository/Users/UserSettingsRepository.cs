using App.Service.DataRepository.Base;
using App.Service.DataRepositoryContracts;
using App.Service.DomainEntities.Users;
using Dapper;
using Signals.Aspects.DI.Attributes;

namespace App.Service.DataRepository.Users
{
    [Export(typeof(IUserSettingsRepository))]
    internal class UserSettingsRepository : BaseDbRepository<UserSettings>, IUserSettingsRepository
    {
        /// <summary>
        /// Return user settings for user
        /// </summary>
        /// <param name="requestingUser"></param>
        /// <returns></returns>
        public UserSettings GetUserSettings(User requestingUser)
        {
            return base.FirstOrDefault(Projection<UserSettings>.Default, x => x.UserId == requestingUser.Id);
        }

        /// <summary>
        /// Update user settings
        /// </summary>
        /// <param name="requestingUser"></param>
        public void UpdateUserSettings(User requestingUser)
        {
            if (requestingUser.Settings == null) return;

            Using(connection =>
            {
                connection.Execute(@"UPDATE USERSETTINGS SET SubscribeToProductEmails = @SubscribeToProductEmails WHERE USERID = @UserId", new
                {
                    requestingUser.Settings.SubscribeToProductEmails,
                    UserId = requestingUser.Id
                });
            });
        }
    }
}