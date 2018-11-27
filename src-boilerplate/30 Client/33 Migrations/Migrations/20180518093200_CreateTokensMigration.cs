using App.Client.Migrations.Base;
using SimpleMigrations;

namespace App.Client.Migrations.Migrations
{
    [Migration(20180518093200, "Create tokens")]
    public class CreateTokensMigration : BaseMigration
    {
        public override void Up()
        {
            Execute(@"CREATE TABLE [dbo].[OnPasswordResetEvent](
	                        [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [CreatedOn] [datetime2](7) NOT NULL,
	                        [Sender] [nvarchar](max) NULL,
	                        [PayloadData] [nvarchar](max) NULL,
	                        [IsProcessed] [bit] NOT NULL,
	                        [ValidUntil] [datetime2](7) NOT NULL,
	                        [Token] [nvarchar](64) NOT NULL,
	                        [UserId] [int] NOT NULL,
                         CONSTRAINT [PK_OnPasswordResetEvent] PRIMARY KEY CLUSTERED
                        (
	                        [Id] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
                        ALTER TABLE [dbo].[OnPasswordResetEvent] ADD  CONSTRAINT [DF_OnPasswordResetEvent_CreateOn]  DEFAULT (getdate()) FOR [CreatedOn];
                        ALTER TABLE [dbo].[OnPasswordResetEvent] ADD  CONSTRAINT [DF_OnPasswordResetEvent_IsProcessed]  DEFAULT ((0)) FOR [IsProcessed];
                        ALTER TABLE [dbo].[OnPasswordResetEvent]  WITH CHECK ADD  CONSTRAINT [FK_OnPasswordResetEvent_User] FOREIGN KEY([UserId]) REFERENCES [dbo].[User] ([Id]);
                        ALTER TABLE [dbo].[OnPasswordResetEvent] CHECK CONSTRAINT [FK_OnPasswordResetEvent_User];
                        ");

            Execute(@"CREATE TABLE [dbo].[OnNewUserRegisterEvent](
	                        [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [CreatedOn] [datetime2](7) NOT NULL,
	                        [Sender] [nvarchar](max) NULL,
	                        [PayloadData] [nvarchar](max) NULL,
	                        [IsProcessed] [bit] NOT NULL,
	                        [ValidUntil] [datetime2](7) NOT NULL,
	                        [Token] [nvarchar](64) NOT NULL,
	                        [UserId] [int] NOT NULL,
                         CONSTRAINT [PK_OnNewUserRegisterEvent] PRIMARY KEY CLUSTERED
                        (
	                        [Id] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
                        ALTER TABLE [dbo].[OnNewUserRegisterEvent] ADD  CONSTRAINT [DF_OnNewUserRegisterEvent_CreateOn]  DEFAULT (getdate()) FOR [CreatedOn];
                        ALTER TABLE [dbo].[OnNewUserRegisterEvent] ADD  CONSTRAINT [DF_OnNewUserRegisterEvent_IsProcessed]  DEFAULT ((0)) FOR [IsProcessed];
                        ALTER TABLE [dbo].[OnNewUserRegisterEvent]  WITH CHECK ADD  CONSTRAINT [FK_OnNewUserRegisterEvent_User] FOREIGN KEY([UserId]) REFERENCES [dbo].[User] ([Id]);
                        ALTER TABLE [dbo].[OnNewUserRegisterEvent] CHECK CONSTRAINT [FK_OnNewUserRegisterEvent_User];
                        ");
        }

        public override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[OnNewUserRegisterEvent] DROP CONSTRAINT [DF_OnNewUserRegisterEvent_CreateOn]");
            Execute(@"ALTER TABLE [dbo].[OnNewUserRegisterEvent] DROP CONSTRAINT [DF_OnNewUserRegisterEvent_IsProcessed]");
            Execute(@"ALTER TABLE [dbo].[OnNewUserRegisterEvent] DROP CONSTRAINT [FK_OnNewUserRegisterEvent_User]");

            Execute(@"DROP TABLE [dbo].[OnNewUserRegisterEvent]");

            Execute(@"ALTER TABLE [dbo].[OnPasswordResetEvent] DROP CONSTRAINT [FK_OnPasswordResetEvent_User]");
            Execute(@"ALTER TABLE [dbo].[OnPasswordResetEvent] DROP CONSTRAINT [DF_OnPasswordResetEvent_IsProcessed]");
            Execute(@"ALTER TABLE [dbo].[OnPasswordResetEvent] DROP CONSTRAINT [DF_OnPasswordResetEvent_CreateOn]");

            Execute(@"DROP TABLE [dbo].[OnPasswordResetEvent]");
        }
    }
}