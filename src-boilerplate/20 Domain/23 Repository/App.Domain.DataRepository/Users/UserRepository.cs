using App.Domain.DataRepository.Base;
using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Common;
using App.Domain.Entities.Users;
using Dapper;
using Signals.Aspects.DI.Attributes;
using System.Collections.Generic;

namespace App.Domain.DataRepository.Users
{
    [Export(typeof(IUserRepository))]
    internal class UserRepository : BaseDbRepository<User>, IUserRepository
    {
        /// <summary>
        /// Get user with password and password hash included
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUserByEmailWithCriticalDataIncluded(string email)
        {
            return base.FirstOrDefault(Projection<User>.Default, x => x.Email == email);
        }

        /// <summary>
        /// Get user with password and password hash included
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetUserByUsernameWithCriticalDataIncluded(string username)
        {
            return base.FirstOrDefault(Projection<User>.Default, user => user.Username == username);
        }

        /// <summary>
        /// Return user by id, allow selection of which properties will be retrieved
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override User GetById(int id)
        {
            var existingUser = base.FirstOrDefault(Projection<User>.Default, user => user.Id == id);
            if (existingUser != null)
            {
                existingUser.Password = null;
                existingUser.PasswordSalt = null;
            }
            return existingUser;
        }

        /// <summary>
        /// Returns user by its email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUserByEmail(string email)
        {
            var existingUser = base.FirstOrDefault(Projection<User>.Default, user => user.Email == email);
            if (existingUser != null)
            {
                existingUser.Password = null;
                existingUser.PasswordSalt = null;
            }
            return existingUser;
        }

        /// <summary>
        /// Returns user by its username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetUserByUsername(string username)
        {
            var existingUser = base.FirstOrDefault(Projection<User>.Default, user => user.Username == username);
            if (existingUser != null)
            {
                existingUser.Password = null;
                existingUser.PasswordSalt = null;
            }
            return existingUser;
        }

        /// <summary>
        /// Returns users by their username
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public List<User> GetUsersById(List<int> userIds)
        {
            var users = base.GetAll(user => userIds.Contains(user.Id));
            for (int i = 0; i < users.Count; i++)
            {
                users[i].Password = null;
                users[i].PasswordSalt = null;
            }
            return users;
        }

        /// <summary>
        /// Search for a content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        public List<User> Search(string content, QueryOptions queryOptions = null)
        {
            var users = base.GetAll(user => user.Email.Contains(content) ||
                                            user.Username.Contains(content) ||
                                            user.Name.Contains(content));

            for (int i = 0; i < users.Count; i++)
            {
                users[i].Password = null;
                users[i].PasswordSalt = null;
            }
            return users;
        }

        /// <summary>
        /// Change current password of the user while the user is logged in
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <param name="newPasswordHash"></param>
        public void UpdatePassword(int userId, string newPassword, string newPasswordHash)
        {
            Using(connection =>
            {
                connection.Execute(@"UPDATE [USER] SET Password = @Password, PasswordSalt = @PasswordSalt WHERE ID = @Id", new
                {
                    Password = newPassword,
                    PasswordSalt = newPasswordHash,
                    Id = userId
                });
            });
        }

        /// <summary>
        /// Update user basic info
        /// </summary>
        /// <param name="requestingUser"></param>
        public void UpdateUserProfile(User requestingUser)
        {
            Using(connection =>
            {
                connection.Execute(@"UPDATE [USER] SET Username = @Username, Name = @Name WHERE ID = @Id", new
                {
                    requestingUser.Username,
                    requestingUser.Name,
                    requestingUser.Id
                });
            });
        }

        /// <summary>
        /// Update user email
        /// </summary>
        /// <param name="requestingUser"></param>
        /// <param name="email"></param>
        public void UpdateUserEmail(User requestingUser, string email)
        {
            Using(connection =>
            {
                connection.Execute(@"UPDATE [USER] SET Email = @Email WHERE ID = @Id", new
                {
                    Email = email,
                    requestingUser.Id
                });
            });
        }

        /// <summary>
        /// Check if an user is exiting (check is done by both username and email)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsExistingUser(string username, string email)
        {
            User existingUser = null;
            if (!string.IsNullOrEmpty(username))
            {
                existingUser = base.FirstOrDefault(Projection<User>.Default, x => x.Username == username);
            }
            else if (!string.IsNullOrEmpty(email))
            {
                existingUser = base.FirstOrDefault(Projection<User>.Default, x => x.Email == email);
            }
            return existingUser != null;
        }

        /// <summary>
        /// Mark that user identity is confirmed
        /// </summary>
        /// <param name="user"></param>
        public void MarUserAsVerified(User user)
        {
            Using(connection =>
            {
                connection.Execute(@"UPDATE [USER] SET IsVerified = @IsVerified WHERE ID = @Id", new
                {
                    IsVerified = true,
                    user.Id
                });
            });
        }
    }
}