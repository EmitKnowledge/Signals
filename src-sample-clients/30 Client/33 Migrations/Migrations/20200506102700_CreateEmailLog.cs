using App.Client.Migrations.Base;
using SimpleMigrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Client.Migrations.Migrations
{
    [Migration(20200506102700)]
    public class CreateEmailLog : BaseMigration
    {
        public override void Up()
        {
            Execute(@"CREATE TABLE [EmailLog] (
	            [Id] [int] IDENTITY(1,1) NOT NULL,
	            [CreatedOn] datetime2(7) NOT NULL,
                [Success] bit NOT NULL,
                [From] nvarchar(MAX) NOT NULL,
                [To] nvarchar(MAX) NOT NULL,
                [Cc] nvarchar(MAX) NULL,
                [Bcc] nvarchar(MAX) NULL,
                [Subject] nvarchar(MAX) NOT NULL,
                [Body] nvarchar(MAX) NOT NULL,
                [Exception] nvarchar(MAX) NULL,
                [SendingReason] nvarchar(MAX) NOT NULL,
                [SendingReasonKey] nvarchar(MAX) NULL,
            CONSTRAINT [PK_EmailLog] PRIMARY KEY CLUSTERED
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
            ");

            Execute(@"ALTER TABLE [dbo].[EmailLog] ADD CONSTRAINT [DF_EmailLog_CreatedOn] DEFAULT (getdate()) FOR [CreatedOn];");
        }

        public override void Down()
        {
            Execute(@"DROP TABLE [dbo].[EmailLog]");
        }
    }
}
