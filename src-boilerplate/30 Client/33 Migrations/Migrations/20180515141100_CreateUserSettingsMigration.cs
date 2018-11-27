using App.Client.Migrations.Base;
using SimpleMigrations;

namespace App.Client.Migrations.Migrations
{
    [Migration(20180515141100, "Create user settings and user images")]
    public class CreateUserSettingsMigration : BaseMigration
    {
        public override void Up()
        {
            Execute(@"CREATE TABLE [dbo].[Currency](
	                        [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [CreatedOn] [datetime2](7) NOT NULL,
	                        [Name] [nvarchar](max) NOT NULL,
	                        [Value] [nvarchar](max) NOT NULL,
	                        [SortPriority] [int] NULL,
	                        [Code] [nvarchar](max) NULL,
                         CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED
                        (
	                        [Id] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
                        ALTER TABLE [dbo].[Currency] ADD  CONSTRAINT [DF_Currency_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn];
                        ALTER TABLE [dbo].[Currency] ADD  CONSTRAINT [DF_Currency_SortPriority]  DEFAULT ((0)) FOR [SortPriority];
                        ");

            Execute(@"SET IDENTITY_INSERT [dbo].[Currency] ON;
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (163, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Euro Member Countries', N'EUR', 0, N'978');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (164, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'United States Dollar', N'USD', 0, N'840');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (165, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Argentina Peso', N'ARS', 0, N'032');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (166, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Australia Dollar', N'AUD', 0, N'036');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (167, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Canada Dollar', N'CAD', 0, N'124');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (168, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Switzerland Franc', N'CHF', 0, N'756');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (169, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Denmark Krone', N'DKK', 0, N'208');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (170, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'United Kingdom Pound', N'GBP', 0, N'GBP');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (171, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Hong Kong Dollar', N'HKD', 0, N'344');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (172, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Indonesia Rupiah', N'IDR', 0, N'360');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (173, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'India Rupee', N'INR', 0, N'356');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (174, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Japan Yen', N'JPY', 0, N'392');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (175, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Korea (South) Won', N'KRW', 0, N'410');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (176, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Macedonia Denar', N'MKD', 0, N'807');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (177, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Mexico Peso', N'MXN', 0, N'484');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (178, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Malaysia Ringgit', N'MYR', 0, N'458');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (179, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Norway Krone', N'NOK', 0, N'578');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (180, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'New Zealand Dollar', N'NZD', 0, N'554');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (181, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Russia Ruble', N'RUB', 0, N'643');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (182, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Saudi Arabia Riyal', N'SAR', 0, N'682');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (183, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Sweden Krona', N'SEK', 0, N'752');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (184, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Turkey Lira', N'TRY', 0, N'949');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (185, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Taiwan New Dollar', N'TWD', 0, N'901');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (186, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'South Africa Rand', N'ZAR', 0, N'710');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (187, CAST(N'2016-12-05T01:43:22.3170000' AS DateTime2), N'Brazil Real', N'BRL', 0, N'986');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (188, CAST(N'2017-04-12T04:57:34.2570000' AS DateTime2), N'United Arab Emirates Dirham', N'AED', 0, N'784');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (189, CAST(N'2017-09-25T00:27:39.5500000' AS DateTime2), N'Serbian dinar', N'RSD', 0, N'941');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (190, CAST(N'2017-09-25T00:28:04.6370000' AS DateTime2), N'Croatian kuna', N'HRK', 0, N'191');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (191, CAST(N'2017-09-25T00:28:27.1370000' AS DateTime2), N'Ukrainian hryvnia', N'UAH', 0, N'980');
                        INSERT [dbo].[Currency] ([Id], [CreatedOn], [Name], [Value], [SortPriority], [Code]) VALUES (192, CAST(N'2017-11-16T23:44:22.9230000' AS DateTime2), N'Colombian peso', N'COP', 0, N'170');
                        SET IDENTITY_INSERT [dbo].[Currency] OFF;");

            Execute(@"CREATE TABLE [dbo].[UserSettings](
	                      [Id] [int] IDENTITY(1,1) NOT NULL,
	                      [CreatedOn] [datetime2](7) NOT NULL,
	                      [UserId] [int] NOT NULL,
                          [SubscribeToProductEmails] [bit] NOT NULL
                       CONSTRAINT [PK_UserSettings] PRIMARY KEY CLUSTERED
                      (
	                      [Id] ASC
                      )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                      ) ON [PRIMARY];

                      ALTER TABLE [dbo].[UserSettings] ADD  CONSTRAINT [DF_UserSettings_SubscribeToProductEmails]  DEFAULT (0) FOR [SubscribeToProductEmails];
                      ALTER TABLE [dbo].[UserSettings] ADD  CONSTRAINT [DF_UserSettings_CreateOn]  DEFAULT (getdate()) FOR [CreatedOn];
                      ALTER TABLE [dbo].[UserSettings] WITH CHECK ADD  CONSTRAINT [FK_UserSettings_User] FOREIGN KEY([UserId]) REFERENCES [dbo].[User] ([Id]);
                      ALTER TABLE [dbo].[UserSettings] CHECK CONSTRAINT [FK_UserSettings_User];
                      ");
        }

        public override void Down()
        {
            Execute(@"DROP TABLE [dbo].[UserSettings]");
            Execute(@"DROP TABLE [dbo].[Currency]");
        }
    }
}