using App.Client.Migrations.Base;
using SimpleMigrations;

namespace App.Client.Migrations.Migrations
{
    [Migration(20180515125800, "Create subscription plans")]
    public class CreateSubscriptionPlanMigration : BaseMigration
    {
        public override void Up()
        {
            Execute(@"CREATE TABLE [dbo].[SubscriptionPlan](
	                        [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [CreatedOn] [datetime2](7) NOT NULL,
	                        [UserId] [int] NOT NULL,
	                        [Identifier] [nvarchar](max) NOT NULL,
	                        [ExternalIdentifier] [nvarchar](max) NOT NULL,
	                        [Version] [int] NOT NULL,
	                        [OrderIdentifier] [nvarchar](32) NULL,
	                        [SaleId] [bigint] NULL,
	                        [Name] [nvarchar](max) NOT NULL,
	                        [MaxClients] [int] NOT NULL,
	                        [Price] [decimal](18, 2) NOT NULL,
	                        [CurrencyCode] [nvarchar](3) NOT NULL,
	                        [Recurrence] [int] NOT NULL,
	                        [TrialStartsOn] [datetime2](7) NOT NULL,
	                        [TrialEndsOn] [datetime2](7) NOT NULL,
	                        [TrialNumberOfDays] [int] NOT NULL,
	                        [IsActive] [bit] NOT NULL,
	                        [CancellatedOn] [datetime2](7) NULL,
	                        [SystemCancelationReason] [int] NULL,
	                        [LastPaymentOn] [datetime2](7) NULL,
	                        [HasDuePayment] [bit] NOT NULL,
	                        [HasDuePaymentSince] [datetime2](7) NULL,
                         CONSTRAINT [PK_SubscriptionPlan] PRIMARY KEY CLUSTERED
                        (
	                        [Id] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

                        ALTER TABLE [dbo].[SubscriptionPlan] ADD  CONSTRAINT [DF_SubscriptionPlan_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn];
                        ALTER TABLE [dbo].[SubscriptionPlan] ADD  CONSTRAINT [DF_SubscriptionPlan_Version]  DEFAULT ((1)) FOR [Version];
                        ALTER TABLE [dbo].[SubscriptionPlan] ADD  CONSTRAINT [DF_SubscriptionPlan_CurrencyCode]  DEFAULT (N'EUR') FOR [CurrencyCode];
                        ALTER TABLE [dbo].[SubscriptionPlan] ADD  CONSTRAINT [DF_SubscriptionPlan_IsActive]  DEFAULT ((1)) FOR [IsActive];
                        ALTER TABLE [dbo].[SubscriptionPlan] ADD  CONSTRAINT [DF_SubscriptionPlan_IsActive1]  DEFAULT ((1)) FOR [HasDuePayment];
                        ALTER TABLE [dbo].[SubscriptionPlan] WITH CHECK ADD  CONSTRAINT [FK_SubscriptionPlan_User] FOREIGN KEY([UserId])  REFERENCES [dbo].[User] ([Id]);
                        ALTER TABLE [dbo].[SubscriptionPlan] CHECK CONSTRAINT [FK_SubscriptionPlan_User];
                        ");
        }

        public override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[SubscriptionPlan] DROP CONSTRAINT [DF_SubscriptionPlan_CreatedOn]");
            Execute(@"ALTER TABLE [dbo].[SubscriptionPlan] DROP CONSTRAINT [DF_SubscriptionPlan_Version]");
            Execute(@"ALTER TABLE [dbo].[SubscriptionPlan] DROP CONSTRAINT [DF_SubscriptionPlan_CurrencyCode]");
            Execute(@"ALTER TABLE [dbo].[SubscriptionPlan] DROP CONSTRAINT [DF_SubscriptionPlan_IsActive]");
            Execute(@"ALTER TABLE [dbo].[SubscriptionPlan] DROP CONSTRAINT [DF_SubscriptionPlan_IsActive1]");
            Execute(@"ALTER TABLE [dbo].[SubscriptionPlan] DROP CONSTRAINT [FK_SubscriptionPlan_User]");

            Execute(@"DROP TABLE [dbo].[SubscriptionPlan]");
        }
    }
}