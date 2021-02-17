using App.Client.Migrations.Base;
using SimpleMigrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Client.Migrations.Migrations
{
    [Migration(20200709135700)]
    public class AlterUserAddUsernameEmailUnique : BaseMigration
    {
        public override void Up()
        {
            Execute($"ALTER TABLE [dbo].[User] ALTER COLUMN [Email] nvarchar(450) NOT NULL;");
            Execute(@"ALTER TABLE [dbo].[User] ADD CONSTRAINT UC_UserEmail UNIQUE (Email);");
            Execute($"ALTER TABLE [dbo].[User] ALTER COLUMN [Username] nvarchar(450) NOT NULL");
            Execute($"ALTER TABLE [dbo].[User] ADD CONSTRAINT UC_UserUsername UNIQUE (Username);");
        }

        public override void Down()
        {
            Execute($"ALTER TABLE [dbo].[User] DROP CONSTRAINT UC_UserEmail;");
            Execute($"ALTER TABLE [dbo].[User] DROP CONSTRAINT UC_UserUsername;");
        }
    }
}
