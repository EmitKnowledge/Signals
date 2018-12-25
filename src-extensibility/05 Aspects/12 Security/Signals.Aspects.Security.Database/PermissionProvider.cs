using Signals.Aspects.Security.Database.Configurations;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Signals.Aspects.Security.Database
{
    /// <summary>
    /// Database permission provider
    /// </summary>
    public class PermissionProvider : IPermissionProvider
    {
        /// <summary>
        /// Database configuration
        /// </summary>
        public DatabaseSecurityConfiguration Configuration { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public PermissionProvider(DatabaseSecurityConfiguration configuration)
        {
            Configuration = configuration;
            CreatePermissionsTableIfNotExist(configuration);
        }

        /// <summary>
        /// Creates or updates permission in the database
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <param name="feature"></param>
        /// <param name="hasAccess"></param>
        public void SetUserPermission(string userIdentifier, string feature, bool hasAccess)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var sqlHasAccess = hasAccess ? 1 : 0;
                var sql =
                    $@"
                        IF EXISTS 
                        (
                            SELECT * 
                            FROM [{Configuration.TableName}] permission 
                            WHERE permission.Feature = @Feature AND
                                  permission.[User] = @User
                        )
                        UPDATE [{Configuration.TableName}] SET HasAccess = {sqlHasAccess}
                        ELSE
                        INSERT INTO [{Configuration.TableName}]([User], Feature, HasAccess) 
                        VALUES (@User, @Feature, {sqlHasAccess})
                    ";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Feature", feature);
                command.Parameters.AddWithValue("User", userIdentifier);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates or updates permission in the database
        /// </summary>
        /// <param name="role"></param>
        /// <param name="feature"></param>
        /// <param name="hasAccess"></param>
        public void SetRolePermission(string role, string feature, bool hasAccess)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                var sqlHasAccess = hasAccess ? 1 : 0;
                var sql =
                    $@"
                        IF EXISTS 
                        (
                            SELECT *
                            FROM [{Configuration.TableName}] permission 
                            WHERE permission.Feature = @Feature AND
                                  permission.[Role] = @Role
                        )
                        UPDATE [{Configuration.TableName}] SET HasAccess = {sqlHasAccess}
                        ELSE
                        INSERT INTO [{Configuration.TableName}]([Role], Feature, HasAccess) 
                        VALUES (@Role, @Feature, {sqlHasAccess})
                    ";
                connection.Open();
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Feature", feature);
                command.Parameters.AddWithValue("Role", role);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Removes user permission from the database
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="feature"></param>
        public void RemoveUserPermission(string userName, string feature)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                var sql =
                    $@"
                        DELETE FROM [{Configuration.TableName}] WHERE [User] = @User AND Feature = @Feature
                    ";
                connection.Open();
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Feature", feature);
                command.Parameters.AddWithValue("User", userName);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Removes role permission from the database
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="feature"></param>
        public void RemoveRolePermission(string roleName, string feature)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                var sql =
                    $@"
                        DELETE FROM [{Configuration.TableName}] WHERE [Role] = @Role AND Feature = @Feature
                    ";
                connection.Open();
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("Feature", feature);
                command.Parameters.AddWithValue("Role", roleName);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Checks if the user has permission for the feature
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="feature"></param>
        /// <param name="userRoles"></param>
        /// <returns></returns>
        public bool HasPermission(string userName, string feature, string[] userRoles)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                // Checks if there is 'grant access' permission entry for the roles and not 'denied access' for the user
                // OR if there is 'grant access' permission entry for the user (without role checking)
                var sql =
                    @"
                        SELECT COUNT(*)
                        FROM Permission p
                        WHERE p.Feature = @Feature AND
                        (
	                        (
		                        p.Role IN (@UserRoles) AND 
		                        p.HasAccess = 1 AND 
		                        (
			                        SELECT COUNT(*) 
			                        FROM Permission p1 
			                        WHERE p1.[User] = @User AND 
			                        p1.Feature = @Feature AND 
			                        p1.HasAccess = 0
		                        ) = 0
	                        ) OR
	                        (p.[User] = @User AND p.HasAccess = 1)
                        )
                    ";

                connection.Open();
                var command = new SqlCommand(sql, connection);
                AddArrayParameters(command, "UserRoles", userRoles);
                command.Parameters.AddWithValue("Feature", feature);
                command.Parameters.AddWithValue("User", userName);

                return (int)command.ExecuteScalar() > 0;
            }
        }

        private SqlParameter[] AddArrayParameters<T>(SqlCommand cmd, string paramNameRoot, IEnumerable<T> values, SqlDbType? dbType = null, int? size = null)
        {
            /* An array cannot be simply added as a parameter to a SqlCommand so we need to loop through things and add it manually. 
             * Each item in the array will end up being it's own SqlParameter so the return value for this must be used as part of the
             * IN statement in the CommandText.
             */
            var parameters = new List<SqlParameter>();
            var parameterNames = new List<string>();
            var paramNbr = 1;
            foreach (var value in values)
            {
                var paramName = string.Format("@{0}{1}", paramNameRoot, paramNbr++);
                parameterNames.Add(paramName);
                SqlParameter p = new SqlParameter(paramName, value);
                if (dbType.HasValue)
                    p.SqlDbType = dbType.Value;
                if (size.HasValue)
                    p.Size = size.Value;
                cmd.Parameters.Add(p);
                parameters.Add(p);
            }

            cmd.CommandText = cmd.CommandText.Replace("@" + paramNameRoot, string.Join(",", parameterNames));

            return parameters.ToArray();
        }

        /// <summary>
        /// Ensures that table for the permission entries exists in the database
        /// </summary>
        /// <param name="databaseConfiguration"></param>
        private void CreatePermissionsTableIfNotExist(DatabaseSecurityConfiguration databaseConfiguration)
        {
            using (var connection = new SqlConnection(databaseConfiguration.ConnectionString))
            {
                connection.Open();

                var sql =
                    $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        WHERE t.name = '{databaseConfiguration.TableName}'
                        ) 
                        CREATE TABLE [{databaseConfiguration.TableName}]
                        (
                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [User] [nvarchar](max) NULL,
	                        [Role] [nvarchar](max) NULL,
	                        [Feature] [nvarchar](max) NOT NULL,
	                        [HasAccess] [bit] NOT NULL
                        )
                    ";

                var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}