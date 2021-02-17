using App.Client.Migrations.Base;
using SimpleMigrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Client.Migrations.Migrations
{
    [Migration(20200507144900)]
    class AlterUserAddLoginAttempts : BaseMigration
    {
        public override void Up()
        {
            Execute($"ALTER TABLE [dbo].[User] ADD LoginAttempts tinyint NULL");
            Execute($"UPDATE [dbo].[User] SET LoginAttempts = 0");
            Execute($"ALTER TABLE [dbo].[User] ALTER COLUMN LoginAttempts tinyint NOT NULL");
        }

        public override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[User] DROP COLUMN LoginAttempts");
        }
    }
}
