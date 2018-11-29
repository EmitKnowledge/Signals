﻿using App.Domain.Entities.Users;
using System.Collections.Generic;

namespace App.Domain.DataRepositoryContracts
{
    public interface IUserExternalConnectionRepository : IRepository<ExternalConnection>
    {
        /// <summary>
        /// Return user settings for user
        /// </summary>
        /// <param name="requestingUser"></param>
        /// <returns></returns>
        List<ExternalConnection> GetExternalConnections(User requestingUser);

        /// <summary>
        /// Update user settings
        /// </summary>
        /// <param name="requestingUser"></param>
        /// <param name="externalConnection"></param>
        int AddExternalConnection(User requestingUser, ExternalConnection externalConnection);

        /// <summary>
        /// Remove an external connection
        /// </summary>
        /// <param name="requestingUser"></param>
        /// <param name="externalConnection"></param>
        void RemoveExternalConnection(User requestingUser, ExternalConnection externalConnection);
    }
}