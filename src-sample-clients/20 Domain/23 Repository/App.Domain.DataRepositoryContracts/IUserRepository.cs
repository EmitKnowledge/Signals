using App.Domain.Entities.Base;
using App.Domain.Entities.Common;
using App.Domain.Entities.Users;
using System.Collections.Generic;

namespace App.Domain.DataRepositoryContracts
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Returns user by its email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        User GetUserByEmail(string email);

        /// <summary>
        /// Returns users sorted
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        PagedResult<User> GetUsersSorted(SortableQueryOptions query);

        /// <summary>
        /// Returns user by its username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        User GetUserByUsername(string username);

        /// <summary>
        /// Returns user by its email or username
        /// </summary>
        /// <param name="emailOrUsername"></param>
        /// <returns></returns>
        User GetUserByEmailOrUsername(string emailOrUsername);

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
        /// Get user by token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        User GetByToken(string token);

        /// <summary>
        /// Get user by token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        User GetByTokenAndUpdateLastAccessDate(string token);

        /// <summary>
        /// Update User last access date
        /// </summary>
        /// <param name="token"></param>
        void UpdateLastAccessDate(string token);

        /// <summary>
        /// Update User last access date and remember me
        /// </summary>
        /// <param name="token"></param>
        /// <param name="rememberMe"></param>
        void UpdateLastAccessDateAndRememberMe(string token, bool rememberMe);

        /// <summary>
        /// Change current password of the user while the user is logged in
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <param name="newPasswordHash"></param>
        /// <param name="passwordResetRequired"></param>
        void UpdatePassword(int userId, string newPassword, string newPasswordHash, bool passwordResetRequired = false);

        /// <summary>
        /// Change current password of the user while the user is logged in and reset login attempts
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <param name="newPasswordHash"></param>
        /// <param name="passwordResetRequired"></param>
        void UpdatePasswordAndResetLoginAttempts(int userId, string newPassword, string newPasswordHash, bool passwordResetRequired = false);

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

        /// <summary>
        /// Update the user token
        /// </summary>
        /// <param name="token"></param>
        void UpdateUserToken(int userId, string token);

        /// <summary>
        /// Update login attempts of estimator
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="numberAttempts"></param>
        void UpdateLoginAttempts(int userId, int numberAttempts);

        /// <summary>
        /// Delete user by id
        /// </summary>
        /// <param name="id"></param>
        void DeleteById(int id);
    }
}