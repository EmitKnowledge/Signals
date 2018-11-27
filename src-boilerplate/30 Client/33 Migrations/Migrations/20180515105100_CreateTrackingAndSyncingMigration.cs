using App.Client.Migrations.Base;
using SimpleMigrations;

namespace App.Client.Migrations.Migrations
{
    [Migration(20180515105100, "Create ActionTrack and SyncLog")]
    public class CreateTrackingAndSyncingMigration : BaseMigration
    {
        public override void Up()
        {
            Execute(@"CREATE TABLE [dbo].[ActionTrack](
	                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                    [CreatedOn] [datetime2](7) NOT NULL,
	                    [Payload] [nvarchar](max) NULL,
	                    [Type] [nvarchar](max) NOT NULL,
	                    [ProcessingStatus] [int] NOT NULL,
	                    [ErrorMessage] [nvarchar](max) NULL,
                     CONSTRAINT [PK_ActionTrack] PRIMARY KEY CLUSTERED
                    (
	                    [Id] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

                    ALTER TABLE [dbo].[ActionTrack] ADD CONSTRAINT [DF_ActionTrack_CreateOn]  DEFAULT (getdate()) FOR [CreatedOn];
                    ");

            Execute(@"CREATE TABLE [dbo].[SentMailTrack](
	                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                    [CreatedOn] [datetime2](7) NOT NULL,
	                    [From] [nvarchar](320) NOT NULL,
	                    [To] [nvarchar](320) NOT NULL,
	                    [ReplyTo] [nvarchar](320) NULL,
	                    [Subject] [nvarchar](max) NULL,
	                    [Body] [nvarchar](max) NULL,
	                    [ErrorMessage] [nvarchar](max) NULL,
	                    [Event] [nvarchar](max) NULL,
                     CONSTRAINT [PK_SentMailTrack] PRIMARY KEY CLUSTERED
                    (
	                    [Id] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

                    ALTER TABLE [dbo].[SentMailTrack] ADD CONSTRAINT [DF_SentMailTrack_CreateOn]  DEFAULT (getdate()) FOR [CreatedOn];
                    ");

            Execute(@"CREATE TABLE [dbo].[SyncLog](
	                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                    [CreatedOn] [datetime2](7) NOT NULL,
	                    [SyncStartedOn] [datetime2](7) NOT NULL,
	                    [SyncEndedOn] [datetime2](7) NOT NULL,
	                    [TotalRecords] [int] NOT NULL,
	                    [RecordsSynced] [int] NOT NULL,
	                    [FailedToSyncRecords] [int] NOT NULL,
	                    [Verbose] [nvarchar](max) NULL,
	                    [SyncType] [nvarchar](max) NOT NULL,
                     CONSTRAINT [PK_SyncLog] PRIMARY KEY CLUSTERED
                    (
	                    [Id] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

                    ALTER TABLE [dbo].[SyncLog] ADD CONSTRAINT [DF_SyncLog_CreateOn]  DEFAULT (getdate()) FOR [CreatedOn];

                    ALTER TABLE [dbo].[SyncLog] ADD CONSTRAINT [DF_SyncLog_TotalRecords]  DEFAULT ((0)) FOR [TotalRecords];

                    ALTER TABLE [dbo].[SyncLog] ADD CONSTRAINT [DF_SyncLog_RecordsSynced]  DEFAULT ((0)) FOR [RecordsSynced];

                    ALTER TABLE [dbo].[SyncLog] ADD CONSTRAINT [DF_SyncLog_FailedToSyncRecords]  DEFAULT ((0)) FOR [FailedToSyncRecords];
                    ");
        }

        public override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[SyncLog] DROP CONSTRAINT [DF_SyncLog_CreateOn]");
            Execute(@"ALTER TABLE [dbo].[SyncLog] DROP CONSTRAINT [DF_SyncLog_TotalRecords]");
            Execute(@"ALTER TABLE [dbo].[SyncLog] DROP CONSTRAINT [DF_SyncLog_RecordsSynced]");
            Execute(@"ALTER TABLE [dbo].[SyncLog] DROP CONSTRAINT [DF_SyncLog_FailedToSyncRecords]");

            Execute(@"ALTER TABLE [dbo].[SentMailTrack] DROP CONSTRAINT [DF_SentMailTrack_CreateOn]");

            Execute(@"ALTER TABLE [dbo].[ActionTrack] DROP CONSTRAINT [DF_ActionTrack_CreateOn]");

            Execute(@"DROP TABLE [dbo].[SyncLog]");
            Execute(@"DROP TABLE [dbo].[SentMailTrack]");
            Execute(@"DROP TABLE [dbo].[ActionTrack]");
        }
    }
}