using App.Domain.Entities.Events.Base;
using App.Domain.Entities.Users;

namespace App.Domain.DataRepositoryContracts
{
    /// <summary>
    /// Token based repository contract
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITokenBasedEventsRepository<T> where T : BaseSystemTokenEvent
    {
        /// <summary>
        /// Remove all previous issued token of the repo type
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="token"></param>
        void RemoveIssuedTokenForUser(int ownerId, string token);

        /// <summary>
        /// Remove all previous issued token of the repo type
        /// </summary>
        /// <param name="token"></param>
        void RemoveIssuedTokenForUser(string token);

        /// <summary>
        /// Create a token in db
        /// </summary>
        /// <param name="eventEntity"></param>
        int CreateToken(T eventEntity);

        /// <summary>
        /// Retrieve a token from db
        /// </summary>
        /// <param name="eventEntity"></param>
        /// <returns></returns>
        T GetToken(T eventEntity);

        /// <summary>
        /// Retrieve a token from db for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        T GetToken(User user);

        /// <summary>
        /// Get token from db
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        T GetToken(string token);

        /// <summary>
        /// Check if token exist in db
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool IsExistingToken(string token);

        /// <summary>
        /// Removes a token from db
        /// </summary>
        /// <param name="eventEntity"></param>
        void InvalidateToken(T eventEntity);
    }
}