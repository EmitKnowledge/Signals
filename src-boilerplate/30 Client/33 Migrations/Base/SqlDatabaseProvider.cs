using Dapper;
using SimpleMigrations;
using System;
using System.Data;
using System.Linq;

namespace App.Client.Migrations.Base
{
    public class SqlDatabaseProvider : IDatabaseProvider<IDbConnection>
    {
        private readonly IDbConnection connection;

        public SqlDatabaseProvider(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IDbConnection BeginOperation()
        {
            return connection;
        }

        public void EndOperation()
        {
        }

        public long EnsurePrerequisitesCreatedAndGetCurrentVersion()
        {
            connection.Execute(@"
                            if not exists (select * from sysobjects where name='SchemaVersion' and xtype='U')
                            BEGIN
                                CREATE TABLE [dbo].[SchemaVersion](
	                                [Id] [int] IDENTITY(1,1) NOT NULL,
	                                [AppliedOn] [datetime2](7) NOT NULL,
	                                [Version] [bigint] NOT NULL,
	                                [Description] nvarchar(MAX) NULL,
                                    CONSTRAINT [PK_SchemaVersion] PRIMARY KEY CLUSTERED
                                    (
	                                    [Id] ASC
                                    )
                                );
                            END");
            return GetCurrentVersion();
        }

        public long GetCurrentVersion()
        {
            // Return 0 if the table has no entries
            var latestOrNull = connection.Query<SchemaVersion>("SELECT * FROM [dbo].[SchemaVersion] ORDER BY Id DESC").FirstOrDefault();
            return latestOrNull?.Version ?? 0;
        }

        public void UpdateVersion(long oldVersion, long newVersion, string newDescription)
        {
            connection.Execute("INSERT INTO [SchemaVersion] (Version, AppliedOn, Description) VALUES (@Version, @AppliedOn, @Description)", new
            {
                Version = newVersion,
                AppliedOn = DateTime.Now,
                Description = newDescription,
            });
        }
    }
}