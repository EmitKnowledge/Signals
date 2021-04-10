using App.Domain.Entities.Common;

namespace App.Domain.DataRepository.Base
{
    internal static class DbRepositoryExtensions
    {
        /// <summary>
        /// Apply paging from query options
        /// </summary>
        /// <param name="querable"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        public static SQLinq.SQLinq<TEntity> ApplyQueryOptions<TEntity>(this SQLinq.SQLinq<TEntity> querable, QueryOptions queryOptions = null)
        {
            if (querable == null || queryOptions == null) return querable;
            querable = querable.Skip(queryOptions.PageSize * (queryOptions.Page - 1))
                               .Take(queryOptions.PageSize);
            return querable;
        }

        /// <summary>
        /// Construct nullable where clause
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="property"></param>
        /// <param name="columnName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetNullableWhere<TEntity>(this TEntity? property, string columnName, string tableName = null, bool ignoreIfNull = false) where TEntity : struct
        {
            tableName = string.IsNullOrEmpty(tableName) ? "" : $"{tableName}.";
            return property.HasValue ? $"{tableName}{columnName} = @{columnName}" : ignoreIfNull ? "1 = 1" : $"{tableName}{columnName} IS NULL";
        }

        /// <summary>
        /// Construct nullable where clause
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="property"></param>
        /// <param name="columnName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetNullableWhere(this object property, string columnName, string tableName = null, bool ignoreIfNull = false)
        {
            tableName = string.IsNullOrEmpty(tableName) ? "" : $"{tableName}.";
            return property != null ? $"{tableName}{columnName} = @{columnName}" : ignoreIfNull ? "1 = 1" : $"{tableName}{columnName} IS NULL";
        }
    }
}