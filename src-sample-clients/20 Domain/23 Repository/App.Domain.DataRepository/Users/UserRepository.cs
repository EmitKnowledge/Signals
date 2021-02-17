using App.Domain.DataRepository.Base;
using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Base;
using App.Domain.Entities.Common;
using App.Domain.Entities.Users;
using Dapper;
using Signals.Aspects.DI.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Domain.DataRepository.Users
{
    [Export(typeof(IUserRepository))]
    internal class UserRepository : BaseDbRepository<User>, IUserRepository
    {
        /// <summary>
        /// Return user by id, allow selection of which properties will be retrieved
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override User GetById(int id)
        {
            var existingUser = base.FirstOrDefault(Projection<User>.Default, user => user.Id == id);
            return existingUser;
        }

        /// <summary>
        /// Get all users sorted
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public PagedResult<User> GetUsersSorted(SortableQueryOptions query)
        {
            return Using(connection =>
            {
                var orderByBuilder = new StringBuilder();
                if (string.IsNullOrEmpty(query.OrderBy))
                {
                    query.OrderBy = @"Username";
                    query.Order = OrderBy.Asc;
                }

                string orderbyField = query.OrderBy;
                var orderby = query.Order.ToString();

                var orderbyStatement = $@"ORDER BY u.{orderbyField}";
                if (query.Order != OrderBy.None)
                {
                    orderbyStatement = $@"ORDER BY u.{orderbyField} {orderby}";
                }

                var pagingStatement = "";
                if (query.PageSize != 0)
                {
                    var offset = query.PageSize * (query.Page - 1);
                    var rows = query.PageSize;
                    pagingStatement = $@"OFFSET {offset} ROWS FETCH NEXT {rows} ROWS ONLY";
                }

                var sql = $@"SELECT u.*
                             FROM [User] u
                             {orderbyStatement}
                             {pagingStatement};

                             SELECT COUNT(*) FROM [User];";

                var reader = connection.QueryMultiple(sql);

                var data = reader.Read<User>().ToList();
                var count = reader.Read<int>().SingleOrDefault();

                return (data, count);
            });
        }

        /// <summary>
        /// Get user by token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public User GetByToken(string token)
        {
            var existingUser = base.FirstOrDefault(Projection<User>.Default, user => user.Token == token);
            return existingUser;
        }

        /// <summary>
        /// Get user by token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public User GetByTokenAndUpdateLastAccessDate(string token)
        {
            return Using(connection =>
            {
                return connection.QueryFirstOrDefault<User>(@"UPDATE [User] SET LastAccessDate = getutcdate() WHERE Token = @Token;
                                                              SELECT TOP 1 * FROM [User] WHERE Token = @Token;",
                new
                {
                    Token = token
                });
            });
        }

        /// <summary>
        /// Update user last access date
        /// </summary>
        /// <param name="token"></param>
        public void UpdateLastAccessDate(string token)
        {
            Using(connection =>
            {
                connection.Execute(@"UPDATE [User] SET LastAccessDate = getutcdate() WHERE Token = @Token;",
                new
                {
                    Token = token
                });
            });
        }

        /// <summary>
        /// Update User last access date and remember me
        /// </summary>
        /// <param name="token"></param>
        /// <param name="rememberMe"></param>
        public void UpdateLastAccessDateAndRememberMe(string token, bool rememberMe)
        {
            Using(connection =>
            {
                connection.Execute(@"UPDATE [User] SET
                                    LastAccessDate = getutcdate(),
                                    RememberMe = @RememberMe
                                    WHERE Token = @Token;",
                    new
                    {
                        RememberMe = rememberMe,
                        Token = token
                    });
            });
        }

        /// <summary>
        /// Returns user by its email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUserByEmail(string email)
        {
            var existingUser = base.FirstOrDefault(Projection<User>.Default, user => user.Email == email);
            return existingUser;
        }

        /// <summary>
        /// Returns user by email or username
        /// </summary>
        /// <param name="emailOrUsername"></param>
        /// <returns></returns>
        public User GetUserByEmailOrUsername(string emailOrUsername)
        {
            var existingUser = base.FirstOrDefault(Projection<User>.Default, user => user.Email == emailOrUsername || user.Username == emailOrUsername);
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
            return users;
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
                connection.Execute(@"UPDATE [User] SET IsVerified = @IsVerified WHERE Id = @Id", new
                {
                    IsVerified = true,
                    user.Id
                });
            });
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
        /// <param name="passwordResetRequired"></param>
        public void UpdatePassword(int userId, string newPassword, string newPasswordHash, bool passwordResetRequired = false)
        {
            Using(connection =>
            {
                connection.Execute(@"UPDATE
                                        [User]
                                     SET
                                        Password = @Password,
                                        PasswordSalt = @PasswordSalt,
                                        PasswordResetRequired = @PasswordResetRequired
                                     WHERE
                                        Id = @Id", new
                {
                    Password = newPassword,
                    PasswordSalt = newPasswordHash,
                    PasswordResetRequired = passwordResetRequired,
                    Id = userId
                });
            });
        }

        /// <summary>
        /// Change current password of the user while the user is logged in and reset login attempts
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <param name="newPasswordHash"></param>
        /// <param name="passwordResetRequired"></param>
        public void UpdatePasswordAndResetLoginAttempts(int userId, string newPassword, string newPasswordHash, bool passwordResetRequired = false)
        {
            Using(connection =>
            {
                connection.Execute(@"UPDATE
                                        [User]
                                     SET
                                        Password = @Password,
                                        PasswordSalt = @PasswordSalt,
                                        PasswordResetRequired = @PasswordResetRequired,
                                        LoginAttempts = 0
                                     WHERE
                                        Id = @Id", new
                {
                    Password = newPassword,
                    PasswordSalt = newPasswordHash,
                    PasswordResetRequired = passwordResetRequired,
                    Id = userId
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
                connection.Execute(@"UPDATE [User] SET Email = @Email WHERE Id = @Id", new
                {
                    Email = email,
                    requestingUser.Id
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
                connection.Execute(@"UPDATE [User]
                                     SET Username = @Username, Name = @Name, Email = @Email, Type = @Type
                                     WHERE Id = @Id",
                new
                {
                    requestingUser.Username,
                    requestingUser.Name,
                    requestingUser.Email,
                    requestingUser.Type,
                    requestingUser.Id
                });
            });
        }

        /// <summary>
        /// Update user token
        /// </summary>
        /// <param name="token"></param>
        public void UpdateUserToken(int userId, string token)
        {
            Using(connection =>
            {
                connection.Execute(@"UPDATE [User] SET Token = @Token WHERE Id = @Id",
                new
                {
                    Token = token,
                    Id = userId
                });
            });
        }

        /// <summary>
        /// Update login attempts of estimator
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="numberAttempts"></param>
        public void UpdateLoginAttempts(int userId, int numberAttempts)
        {
            Using(connection =>
            {
                connection.Execute(@"UPDATE [User] SET LoginAttempts = @LoginAttempts WHERE Id = @Id",
                new
                {
                    LoginAttempts = numberAttempts,
                    Id = userId
                });
            });
        }

        /// <summary>
        /// Delete user by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteById(int id)
        {
            Using(connection =>
            {
                connection.Execute(@"DELETE FROM [User] WHERE Id = @Id",
                    new
                    {
                        Id = id
                    });
            });
        }
    }
}