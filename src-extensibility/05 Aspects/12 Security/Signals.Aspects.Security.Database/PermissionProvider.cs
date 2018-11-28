using Signals.Aspects.Security.Database.Configurations;
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
                        IF TYPE_ID(N'UserRolesType') IS NULL
                        CREATE TYPE [UserRolesType] AS TABLE
                        (
                            Name NVARCHAR(MAX)
                        )

                        SELECT COUNT(*)
                        FROM Permission p
                        WHERE p.Feature = @Feature AND
                        (
	                        (
		                        p.Role IN (SELECT Name FROM @UserRoles) AND 
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

                // Create data table and insert the user roles
                var userRolesTable = new DataTable("UserRoles");
                userRolesTable.Columns.Add("Name", typeof(string));
                foreach (var userRole in userRoles)
                    userRolesTable.Rows.Add(userRole);

                // Add userRolesTable as sql parameter
                var param = new SqlParameter
                {
                    Value = userRolesTable,
                    ParameterName = "UserRoles",
                    TypeName = "[UserRolesType]"
                };
                command.Parameters.Add(param);
                command.Parameters.AddWithValue("Feature", feature);
                command.Parameters.AddWithValue("User", userName);

                return (int)command.ExecuteScalar() > 0;
            }
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