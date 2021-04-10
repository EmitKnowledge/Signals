using App.Domain.Configuration;
using App.Domain.DataRepository._Db;
using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Base;
using App.Domain.Entities.Common;
using Dapper;
using DapperExtensions;
using NodaTime;
using SQLinq;
using SQLinq.Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace App.Domain.DataRepository.Base
{
    internal class BaseDbRepository<T> : IRepository<T> where T : class, IBaseDomainEntity<int>, new()
    {
        /// <summary>
        /// Connection string
        /// </summary>
        protected string ConnectionString { get; set; }

        /// <summary>
        /// STATIC CTOR
        /// </summary>
        static BaseDbRepository()
        {
            // set mappings source assemlby
            var assembly = Assembly.GetExecutingAssembly();
            DapperExtensions.DapperExtensions.SetMappingAssemblies(new List<Assembly> { assembly });
            // custom mapping
            //SqlMapper.AddTypeMap(typeof(EmitDateTime), DbType.DateTime);
            //SqlMapper.AddTypeMap(typeof(EmitDateTime), DbType.DateTime2);
            SqlMapper.AddTypeHandler(typeof(Instant), new InstantTimeHandler());
        }

        /// <summary>
        /// CTOR
        /// </summary>
        public BaseDbRepository()
        {
            ConnectionString = DomainConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.ConnectionString;
        }

        /// <summary>
        /// Insert entity in db
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Insert(T entity)
        {
            return Using(connection =>
                {
                    int id = connection.Insert(entity);
                    entity.Id = id;
                    return id;
                });
        }

        /// <summary>
        /// Insert entities in db
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public void Insert(List<T> entities)
        {
            Using(connection =>
            {
                connection.Insert(entities);
            });
        }

        /// <summary>
        /// Delete an entity from db
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void Delete(T entity)
        {
            Using(connection =>
            {
                connection.Delete(entity);
            });
        }

        /// <summary>
        /// Delete db entities that match the conditions
        /// </summary>
        /// <param name="where"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public void Delete(string @where, object paramValues)
        {
            Using(connection =>
                {
                    var query = string.Concat(@"DELETE FROM [" + typeof(T).Name + "] WHERE ", @where);
                    connection.Execute(query, paramValues);
                });
        }

        /// <summary>
        /// Search for entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual List<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return Using(connection =>
                {
                    var query = new SQLinq<T>().Where(predicate);
                    var data = connection.Query(query);
                    return data.ToList();
                });
        }

        /// <summary>
        /// Return all entities from db collection
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetAll(Expression<Func<T, object>> @select = null, QueryOptions queryOptions = null)
        {
            var query = new SQLinq<T>();
            if (select != null)
            {
                query = query.Select(@select);
            }
            return Using(connection =>
                {
                    var data = connection.Query(query.ApplyQueryOptions(queryOptions));
                    return data.ToList();
                });
        }

        /// <summary>
        /// Return an entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(int id)
        {
            return Using(connection =>
                {
                    var query = from e in new SQLinq<T>() where e.Id == id select e;
                    var result = connection.Query(query).FirstOrDefault();
                    return result;
                });
        }

        /// <summary>
        /// Return an entity that match the criteria
        /// </summary>
        /// <param name="select"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual T FirstOrDefault(Expression<Func<T, object>> @select, Expression<Func<T, bool>> predicate)
        {
            return Using(connection =>
                {
                    var query = new SQLinq<T>();
                    if (select != null)
                    {
                        query = query.Select(@select);
                    }
                    if (predicate != null)
                    {
                        query = query.Where(predicate);
                    }
                    var result = connection.Query(query).FirstOrDefault();
                    return result;
                });
        }

        /// <summary>
        /// Open a connection
        /// </summary>
        /// <returns></returns>
        protected virtual IDbConnection GetOpenConnection()
        {
            var scsb = new SqlConnectionStringBuilder(ConnectionString);
            scsb.MultipleActiveResultSets = true;
            var cs = scsb.ConnectionString;
            var connection = new SqlConnection(cs);
            return connection;
        }

        /// <summary>
        /// No value context using
        /// </summary>
        /// <param name="contextAction"></param>
        protected void Using(Action<IDbConnection> contextAction)
        {
            using (var connection = GetOpenConnection())
            {
                connection.Open();
                contextAction(connection);
                connection.Close();
            }
        }

        /// <summary>
        /// Return value context using
        /// </summary>
        /// <param name="contextAction"></param>
        /// <returns></returns>
        protected TK Using<TK>(Func<IDbConnection, TK> contextAction)
        {
            using (var connection = GetOpenConnection())
            {
                connection.Open();
                var result = contextAction(connection);
                connection.Close();
                return result;
            }
        }
    }

    /// <summary>
    /// Default projection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Projection<T>
    {
        public static readonly Expression<Func<T, object>> Default = null;
    }
}