using System.Collections.Generic;
using App.Service.DataRepository.Base;
using App.Service.DataRepositoryContracts;
using App.Service.DomainEntities.Users;
using Dapper;
using DapperExtensions;
using Signals.Aspects.DI.Attributes;

namespace App.Service.DataRepository.Users
{
    [Export(typeof(IUserExternalConnectionRepository))]
    internal class UserExternalConnectionRepository : BaseDbRepository<ExternalConnection>, IUserExternalConnectionRepository
    {
        /// <summary>
        /// Return user settings for user
        /// </summary>
        /// <param name="requestingUser"></param>
        /// <returns></returns>
        public List<ExternalConnection> GetExternalConnections(User requestingUser)
        {
            return base.GetAll(x => x.UserId == requestingUser.Id);
        }

        /// <summary>
        /// Update user settings
        /// </summary>
        /// <param name="requestingUser"></param>
        /// <param name="externalConnection"></param>
        public int AddExternalConnection(User requestingUser, ExternalConnection externalConnection)
        {
            return Using(connection =>
            {
                externalConnection.UserId = requestingUser.Id;
                int id = connection.Insert(externalConnection);
                externalConnection.Id = id;
                return id;
            });
        }

        /// <summary>
        /// Remove an external connection
        /// </summary>
        /// <param name="requestingUser"></param>
        /// <param name="externalConnection"></param>
        public void RemoveExternalConnection(User requestingUser, ExternalConnection externalConnection)
        {
            Using(connection =>
            {
                connection.Execute(@"DELETE FROM [ExternalConnection] WHERE UserId = @UserId AND Provider = @Provider", new
                {
                    UserId = requestingUser.Id,
                    Provider = externalConnection.Provider
                });
            });
        }
    }
}
