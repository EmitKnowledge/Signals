using Signals.Aspects.Security.Database;
using Signals.Aspects.Security.Database.Configurations;
using System.Data.SqlClient;
using Xunit;

namespace Signals.Tests.Security
{
    public class PermissionTests
    {
        private readonly DatabaseSecurityConfiguration _databaseConfiguration;
        private readonly PermissionProvider _permissionProvider;

        public PermissionTests()
        {
            _databaseConfiguration = new DatabaseSecurityConfiguration
            {
                ConnectionString = "Server=sql.emitknowledge.com;Database=app.db;User Id=appusr;Password=FYGncRXGySXDz6RFNg2e;"
            };
            _permissionProvider = new PermissionProvider(_databaseConfiguration);

            // Clear the database
            ClearDb();
        }

        [Fact]
        public void The_User_Should_Have_Access_If_They_Have_Granted_Permission()
        {
            // Arrange
            var feature = "CreateContract";
            var user = "aleksandar";
            var userRoles = new[] { "Admin" };

            // Act
            _permissionProvider.SetUserPermission(user, feature, true);
            var hasAccess = _permissionProvider.HasPermission(user, feature, userRoles);

            // Assert
            Assert.True(hasAccess);
        }

        [Fact]
        public void The_User_Should_Not_Have_Access_If_They_Have_Denied_Permission()
        {
            // Arrange
            var feature = "CreateContract";
            var user = "aleksandar";
            var userRoles = new[] { "Admin" };

            // Act
            _permissionProvider.SetUserPermission(user, feature, false);
            var hasAccess = _permissionProvider.HasPermission(user, feature, userRoles);

            // Assert
            Assert.False(hasAccess);
        }

        [Fact]
        public void The_User_Should_Have_Access_If_They_Have_Granted_Permission_And_Their_Roles_Dont()
        {
            // Arrange
            var feature = "CreateContract";
            var user = "aleksandar";
            var role = "Admin";
            var userRoles = new[] { role };

            // Act
            _permissionProvider.SetUserPermission(user, feature, true);
            _permissionProvider.SetRolePermission(role, feature, false);
            var hasAccess = _permissionProvider.HasPermission(user, feature, userRoles);

            // Assert
            Assert.True(hasAccess);
        }

        [Fact]
        public void The_User_Should_Not_Have_Access_If_They_Have_Denied_Permission_Even_If_Their_Roles_Have_Granted_Permission()
        {
            // Arrange
            var feature = "CreateContract";
            var user = "aleksandar";
            var role = "Admin";
            var userRoles = new[] { role };

            // Act
            _permissionProvider.SetUserPermission(user, feature, false);
            _permissionProvider.SetRolePermission(role, feature, true);
            var hasAccess = _permissionProvider.HasPermission(user, feature, userRoles);

            // Assert
            Assert.False(hasAccess);
        }

        [Fact]
        public void The_User_Should_Have_Access_If_Their_Roles_Do()
        {
            // Arrange
            var feature = "CreateContract";
            var user = "aleksandar";
            var role = "Admin";
            var userRoles = new[] { role };

            // Act
            _permissionProvider.SetRolePermission(role, feature, true);
            var hasAccess = _permissionProvider.HasPermission(user, feature, userRoles);

            // Assert
            Assert.True(hasAccess);
        }

        [Fact]
        public void The_User_Should_Not_Have_Access_If_Their_Roles_Dont()
        {
            // Arrange
            var feature = "CreateContract";
            var user = "aleksandar";
            var role = "Admin";
            var userRoles = new[] { role };

            // Act
            _permissionProvider.SetRolePermission(role, feature, false);
            var hasAccess = _permissionProvider.HasPermission(user, feature, userRoles);

            // Assert
            Assert.False(hasAccess);
        }

        private void ClearDb()
        {
            using (var conn = new SqlConnection(_databaseConfiguration.ConnectionString))
            {
                var sql = $"DELETE FROM [{_databaseConfiguration.TableName}]";
                conn.Open();
                new SqlCommand(sql, conn).ExecuteNonQuery();
            }
        }
    }
}
