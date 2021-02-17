using App.Client.Migrations.Base;
using App.Domain.Configuration;
using SimpleMigrations;

namespace App.Client.Migrations.Migrations
{
    [Migration(20200505140400)]
    public class AlterUserAddPasswordResetRequired : BaseMigration
    {
        public override void Up()
        {
            Execute($"ALTER TABLE [dbo].[User] ADD PasswordResetRequired BIT NULL");
            Execute($"UPDATE [dbo].[User] SET PasswordResetRequired = 0");
            Execute($"ALTER TABLE [dbo].[User] ALTER COLUMN PasswordResetRequired BIT NOT NULL");
        }

        public override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[User] DROP COLUMN PasswordResetRequired");
        }
    }
}