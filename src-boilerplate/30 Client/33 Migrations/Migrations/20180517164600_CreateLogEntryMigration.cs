using App.Client.Migrations.Base;
using SimpleMigrations;

namespace App.Client.Migrations.Migrations
{
    [Migration(20180517164600, "Create log entry")]
    public class CreateLogEntryMigration : BaseMigration
    {
        public override void Up()
        {
            Execute(@"CREATE TABLE [dbo].[LogEntry](
	                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                    [CreatedOn] [datetime2](7) NOT NULL,
	                    [Level] [nvarchar](60) NULL,
	                    [ErrorGroup] [nvarchar](60) NULL,
	                    [ErrorCode] [nvarchar](60) NULL,
	                    [Origin] [nvarchar](120) NULL,
	                    [Action] [nvarchar](600) NULL,
	                    [ActionFilePath] [nvarchar](max) NULL,
	                    [ActionSourceLineNumber] [nvarchar](30) NULL,
	                    [Message] [nvarchar](max) NULL,
	                    [ExceptionMessage] [nvarchar](max) NULL,
	                    [UserId] [int] NULL,
	                    [Payload] [nvarchar](max) NULL,
                     CONSTRAINT [PK_LogEntry] PRIMARY KEY CLUSTERED
                    (
	                    [Id] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

                    ALTER TABLE [dbo].[LogEntry] ADD  CONSTRAINT [DF_LogEntry_CreatedOn] DEFAULT (getdate()) FOR [CreatedOn];
                    ALTER TABLE [dbo].[LogEntry] WITH CHECK ADD CONSTRAINT [FK_LogEntry_User] FOREIGN KEY([UserId]) REFERENCES [dbo].[User] ([Id]);
                    ALTER TABLE [dbo].[LogEntry] CHECK CONSTRAINT [FK_LogEntry_User];");
        }

        public override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[LogEntry] DROP CONSTRAINT [DF_LogEntry_CreatedOn]");
            Execute(@"ALTER TABLE [dbo].[LogEntry] DROP CONSTRAINT [FK_LogEntry_User]");

            Execute(@"DROP TABLE [dbo].[LogEntry]");
        }
    }
}