using App.Client.Migrations.Base;
using SimpleMigrations;

namespace App.Client.Migrations.Migrations
{
    [Migration(20180515103900, "Create user")]
    public class CreateUserMigration : BaseMigration
    {
        public override void Up()
        {
            Execute(@"CREATE TABLE [User] (
	            [Id] [int] IDENTITY(1,1) NOT NULL,
	            CreatedOn datetime2(7) NOT NULL,
                Name nvarchar(MAX) NULL,
                Description nvarchar(MAX) NULL,
                Email nvarchar(MAX) NOT NULL,
                Username nvarchar(MAX) NOT NULL,
                Password nvarchar(MAX) NOT NULL,
	            IsVerified [bit] NOT NULL,
                PasswordSalt nvarchar(MAX) NOT NULL,
                Type int NOT NULL,
            CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
            ");

            Execute(@"ALTER TABLE [dbo].[User] ADD CONSTRAINT [DF_User_IsVerified]  DEFAULT ((0)) FOR [IsVerified];");
            Execute(@"ALTER TABLE [dbo].[User] ADD CONSTRAINT [DF_User_CreateOn] DEFAULT (getdate()) FOR [CreatedOn];");
        }

        public override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[User] DROP CONSTRAINT [DF_User_IsVerified]");
            Execute(@"ALTER TABLE [dbo].[User] DROP CONSTRAINT [DF_User_CreateOn]");

            Execute(@"DROP TABLE [dbo].[User]");
        }
    }
}