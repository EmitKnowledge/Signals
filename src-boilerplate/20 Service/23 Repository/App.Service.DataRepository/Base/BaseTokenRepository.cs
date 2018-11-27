using App.Service.DataRepositoryContracts;
using App.Service.DomainEntities.Events.Base;
using App.Service.DomainEntities.Users;
using NodaTime;

namespace App.Service.DataRepository.Base
{
    /// <summary>
    /// Generic token repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class BaseTokenRepository<T> : BaseDbRepository<T>, ITokenBasedEventsRepository<T> where T : BaseSystemTokenEvent, new()
    {
        /// <summary>
        /// Remove all previous issued token of the repo type
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="token"></param>
        public virtual void RemoveIssuedTokenForUser(int ownerId, string token)
        {
            base.Delete(@"UserId = @ownerId AND Token = @token", new { ownerId, token });
        }

        /// <summary>
        /// Remove all previous issued token of the repo type
        /// </summary>
        /// <param name="token"></param>
        public virtual void RemoveIssuedTokenForUser(string token)
        {
            base.Delete(@"Token = @token", new { token });
        }

        /// <summary>
        /// Create a token in db
        /// </summary>
        /// <param name="eventEntity"></param>
        public virtual int CreateToken(T eventEntity)
        {
            return base.Insert(eventEntity);
        }

        /// <summary>
        /// Retrieve a token from db
        /// </summary>
        /// <param name="eventEntity"></param>
        /// <returns></returns>
        public virtual T GetToken(T eventEntity)
        {
            if (eventEntity == null || string.IsNullOrEmpty(eventEntity.Token)) return null;
            var token = Using(context => base.FirstOrDefault(Projection<T>.Default, x => x.Token == eventEntity.Token));
            return token;
        }

        /// <summary>
        /// Retrieve a token from db for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public T GetToken(User user)
        {
            if (user == null) return null;
            var token = Using(context => base.FirstOrDefault(Projection<T>.Default, x => x.UserId == user.Id));
            return token;
        }

        /// <summary>
        /// Get token from db
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual T GetToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            var existingToken = Using(connection => base.FirstOrDefault(Projection<T>.Default, x => x.Token == token));
            return existingToken;
        }

        /// <summary>
        /// Check if token exist in db
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual bool IsExistingToken(string token)
        {
            var tokenEvent = GetToken(token);

            // check if existing
            if (tokenEvent == null) return false;
            // check if expired
            if (tokenEvent.ValidUntil < SystemClock.Instance.GetCurrentInstant())
            {
                InvalidateToken(tokenEvent);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes a token from db
        /// </summary>
        /// <param name="eventEntity"></param>
        public virtual void InvalidateToken(T eventEntity)
        {
            if (eventEntity == null || string.IsNullOrEmpty(eventEntity.Token)) return;
            RemoveIssuedTokenForUser(eventEntity.Token);
        }
    }
}
