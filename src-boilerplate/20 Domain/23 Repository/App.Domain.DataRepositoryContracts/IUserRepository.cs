using App.Domain.Entities.Common;
using App.Domain.Entities.Users;
using System.Collections.Generic;

namespace App.Domain.DataRepositoryContracts
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Get user with password and password hash included
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        User GetUserByEmailWithCriticalDataIncluded(string email);

        /// <summary>
        /// Get user with password and password hash included
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        User GetUserByUsernameWithCriticalDataIncluded(string username);

        /// <summary>
        /// Returns user by its email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        User GetUserByEmail(string email);

        /// <summary>
        /// Returns user by its username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        User GetUserByUsername(string username);

        /// <summary>
        /// Returns users by their username
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        List<User> GetUsersById(List<int> userIds);

        /// <summary>
        /// Search for a content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        List<User> Search(string content, QueryOptions queryOptions = null);

        /// <summary>
        /// Change current password of the user while the user is logged in
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <param name="newPasswordHash"></param>
        void UpdatePassword(int userId, string newPassword, string newPasswordHash);

        /// <summary>
        /// Updates general user data.
        /// </summary>
        /// <param name="user"></param>
        void UpdateUserProfile(User user);

        /// <summary>
        /// Check if an user is exiting (check is done by both username and email)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        bool IsExistingUser(string username, string email);

        /// <summary>
        /// Mark that user identity is confirmed
        /// </summary>
        /// <param name="user"></param>
        void MarUserAsVerified(User user);
    }
}