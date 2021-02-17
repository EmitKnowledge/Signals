using App.Client.Migrations.Base;
using SimpleMigrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Client.Migrations.Migrations
{
    [Migration(20200507134100)]
    class AlterUserAddRememberMe : BaseMigration
    {
        public override void Up()
        {
            Execute($"ALTER TABLE [dbo].[User] ADD RememberMe BIT NULL");
            Execute($"UPDATE [dbo].[User] SET RememberMe = 0");
            Execute($"ALTER TABLE [dbo].[User] ALTER COLUMN RememberMe BIT NOT NULL");
        }

        public override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[User] DROP COLUMN RememberMe");
        }
    }
}
