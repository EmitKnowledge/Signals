using App.Client.Migrations.Base;
using App.Domain.Configuration;
using SimpleMigrations;

namespace App.Client.Migrations.Migrations
{
    [Migration(20200504133100)]
    public class AlterUserAddToken : BaseMigration
    {
        public override void Up()
        {
            Execute($"ALTER TABLE [dbo].[User] ADD Token NVARCHAR({DomainConfiguration.Instance.SecurityConfiguration.TokenLength + 1}) NOT NULL");
        }

        public override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[User] DROP COLUMN Token");
        }
    }
}