using App.Service.DomainEntities.Users;

namespace App.Service.DataRepositoryContracts
{
    public interface IUserSettingsRepository : IRepository<UserSettings>
    {
        /// <summary>
        /// Return user settings for user
        /// </summary>
        /// <param name="requestingUser"></param>
        /// <returns></returns>
        UserSettings GetUserSettings(User requestingUser);

        /// <summary>
        /// Update user settings
        /// </summary>
        /// <param name="requestingUser"></param>
        void UpdateUserSettings(User requestingUser);
    }
}