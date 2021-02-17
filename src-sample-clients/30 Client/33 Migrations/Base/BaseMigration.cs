using Dapper;
using SimpleMigrations;
using System;
using System.Data;
using System.Transactions;

namespace App.Client.Migrations.Base
{
    public abstract class BaseMigration : IMigration<IDbConnection>
    {
        protected IDbConnection Connection { get; set; }

        protected IMigrationLogger Logger { get; set; }

        public abstract void Up();

        public abstract void Down();

        public void Execute(string sql)
        {
            Logger.LogSql(sql);
            Connection.Execute(sql);
        }

        void IMigration<IDbConnection>.RunMigration(MigrationRunData<IDbConnection> data)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(10)))
            {
                Connection = data.Connection;
                Logger = data.Logger;

                if (data.Direction == MigrationDirection.Up)
                {
                    Up();
                    Down();
                    Up();
                }
                else
                {
                    Down();
                }

                scope.Complete();
            }
        }
    }
}