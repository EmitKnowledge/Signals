using Signals.Aspects.Security.Database.Configurations;
using System.Data;
using System.Data.SqlClient;

namespace Signals.Aspects.Security.Database
{
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
                            FROM dbo.[{Configuration.TableName}] permission 
                            WHERE permission.Feature = @Feature AND
                                  permission.[User] = @User
                        )
                        UPDATE dbo.[{Configuration.TableName}] SET HasAccess = {sqlHasAccess}
                        ELSE
                        INSERT INTO dbo.[{Configuration.TableName}]([User], Feature, HasAccess) 
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
                            FROM dbo.[{Configuration.TableName}] permission 
                            WHERE permission.Feature = @Feature AND
                                  permission.[Role] = @Role
                        )
                        UPDATE dbo.[{Configuration.TableName}] SET HasAccess = {sqlHasAccess}
                        ELSE
                        INSERT INTO dbo.[{Configuration.TableName}]([Role], Feature, HasAccess) 
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
                        DELETE FROM dbo.[{Configuration.TableName}] WHERE [User] = @User AND Feature = @Feature
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
                        DELETE FROM dbo.[{Configuration.TableName}] WHERE [Role] = @Role AND Feature = @Feature
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
                        CREATE TYPE [dbo].[UserRolesType] AS TABLE
                        (
                            Name NVARCHAR(MAX)
                        )

                        SELECT COUNT(*)
                        FROM dbo.Permission p
                        WHERE p.Feature = @Feature AND
                        (
	                        (
		                        p.Role IN (SELECT Name FROM @UserRoles) AND 
		                        p.HasAccess = 1 AND 
		                        (
			                        SELECT COUNT(*) 
			                        FROM dbo.Permission p1 
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
                    TypeName = "[dbo].[UserRolesType]"
                };
                command.Parameters.Add(param);
                command.Parameters.AddWithValue("Feature", feature);
                command.Parameters.AddWithValue("User", userName);

                return (int)command.ExecuteScalar() > 0;
            }
        }
    }
}