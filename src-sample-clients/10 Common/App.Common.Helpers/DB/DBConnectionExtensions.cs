using NodaTime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace App.Common.Helpers.DB
{
    public static class DBConnectionExtensions
    {
        /// <summary>
        /// Generate sql ready to paste in sql studio for profiling
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="property"></param>
        /// <param name="columnName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetProfilingSql<TEntity>(this IDbConnection connection, string sql, TEntity obj)
        {
            var declareImports = "";

            List<Tuple<string, object, Type>> props = new List<Tuple<string, object, Type>>();

            if (obj is ExpandoObject)
            {
                foreach (var e in obj as ExpandoObject)
                {
                    props.Add(new Tuple<string, object, Type>(e.Key, e.Value, e.Value?.GetType()));
                }
            }
            else
            {
                props = obj.GetType().GetProperties().Select(x => new Tuple<string, object, Type>(x.Name, x.GetValue(obj), x.PropertyType)).ToList();
            }

            foreach (var prop in props)
            {
                var name = prop.Item1;
                var val = prop.Item2;
                var type = prop.Item3;

                if (type == null) continue;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = Nullable.GetUnderlyingType(type);
                }

                var sqlVal = "NULL";
                var sqlType = "";
                var declareSql = "";

                if (type == typeof(int) || type.IsEnum)
                {
                    sqlType = "int";
                    sqlVal = val == null ? "NULL" : ((int)val).ToString();
                }
                else if (type == typeof(string))
                {
                    sqlType = "nvarchar(MAX)";
                    sqlVal = val == null ? "NULL" : $"'{val}'";
                }
                else if (type == typeof(DateTime))
                {
                    var d = val as DateTime?;
                    sqlType = "datetime2(7)";
                    sqlVal = val == null ? "NULL" : $"'{d?.ToString("yyyy-MM-ddTHH:mm:ssZ")}'";
                }
                else if (type == typeof(Instant))
                {
                    var d = val as Instant?;
                    sqlType = "datetime2(7)";
                    sqlVal = val == null ? "NULL" : $"'{d?.ToDateTimeUtc().ToString("yyyy-MM-ddTHH:mm:ssZ")}'";
                }
                else if (type == typeof(double) || type == typeof(decimal))
                {
                    sqlType = "decimal(18,4)";
                    sqlVal = val == null ? "NULL" : val.ToString();
                }

                declareSql = $"DECLARE @{name} {sqlType} = {sqlVal}";
                declareImports += $"{declareSql};\n";
            }

            return $"{declareImports} \n {sql}";
        }
    }
}