//using MongoDB.Bson;
//using App.Common.Base.Repository;

namespace App.Service.DomainEntities.Base
{
    public static class BaseEntityExtensions
    {
        /**
         * UNCOMMENT FOR MONGODB
        /// <summary>
        /// Create unique id for a given base entity
        /// </summary>
        /// <param name="baseEntity"></param>
        public static void CreateId<TBaseEntity>(this TBaseEntity baseEntity) where TBaseEntity : IMongoDbEntity
        {
            baseEntity.Id = ObjectId.GenerateNewId();
        }
        **/
    }
}