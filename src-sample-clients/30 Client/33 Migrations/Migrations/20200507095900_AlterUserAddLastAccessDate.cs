using App.Client.Migrations.Base;
using App.Domain.Configuration;
using SimpleMigrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Client.Migrations.Migrations
{
    [Migration(20200507095900)]
    public class AlterUserAddLastAccessDate : BaseMigration
    {
        public override void Up()
        {
            Execute($"ALTER TABLE [dbo].[User] ADD LastAccessDate DateTime NULL");
        }

        public override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[User] DROP COLUMN LastAccessDate");
        }
    }
}
