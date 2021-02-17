using App.Domain.Entities.Base;
using App.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace App.Domain.DataRepositoryContracts
{
    /// <summary>
    /// Generic db repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : class, IBaseDomainEntity<int>, new()
    {
        /// <summary>
        /// Insert entity in db
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Insert(TEntity entity);

        /// <summary>
        /// Insert entity in db
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        void Insert(List<TEntity> entities);

        /// <summary>
        /// Delete an entity from db
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Delete(TEntity entity);

        /// <summary>
        /// Delete db entities that match the conditions
        /// </summary>
        /// <param name="where"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        void Delete(string where, object paramValues);

        /// <summary>
        /// Search for entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Return all entities from db collection
        /// </summary>
        /// <returns></returns>
        List<TEntity> GetAll(Expression<Func<TEntity, object>> @select = null, QueryOptions queryOptions = null);

        /// <summary>
        /// Return an entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetById(int id);

        /// <summary>
        /// Return an entity that match the criteria
        /// </summary>
        /// <param name="select"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, object>> @select, Expression<Func<TEntity, bool>> predicate);
    }
}