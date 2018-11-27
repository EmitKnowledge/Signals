using App.Client.Migrations.Base;
using SimpleMigrations;

namespace App.Client.Migrations.Migrations
{
    [Migration(20180528131200, "Add column to enable locking user account")]
    public class AlterUserLockAccount : BaseMigration
    {
        public override void Up()
        {
            Execute(@"ALTER TABLE [User] ADD IsLocked bit NULL;");
            Execute(@"ALTER TABLE [User] ADD CONSTRAINT DF_User_IsLocked DEFAULT 0 FOR IsLocked;");
            Execute(@"UPDATE [User] SET [User].[IsLocked] = 0;");
            Execute(@"ALTER TABLE [User] ALTER COLUMN IsLocked bit NOT NULL;");
        }

        public override void Down()
        {
            Execute(@"ALTER TABLE [User] ALTER COLUMN IsLocked bit NULL;");
            Execute(@"ALTER TABLE [User] DROP CONSTRAINT DF_User_IsLocked;");
            Execute(@"ALTER TABLE [User] DROP COLUMN IsLocked;");
        }
    }
}