using App.Client.Migrations.Base;
using SimpleMigrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Client.Migrations.Migrations
{
    [Migration(20200626103800)]
    public class AddUser : BaseMigration
    {
        public override void Up()
        {
            Execute(@"INSERT INTO [User] 
                    (Name, Description, Email, Username, Password, IsVerified, PasswordSalt, Type, Token, 
                    PasswordResetRequired, LastAccessDate, RememberMe, LoginAttempts) VALUES 
                    ('Admin', 'Desc', 'gjozev@gmail.com', 'adminusr', 
                    '5233d01f9d05aab059a40055a97650515227336ffc47dedbec9b128e88ac0d33', 0, 
                    '3zhEWOAHPBYZYQ9sBD56av0pR9WyNWy4', 1,
                    'qdDoaQf6wdcm5uVeKAGaaVXqoeuKIflKOF3E4AsWqDcrt7I31xa9q0RVE2Z82B9z3lXORwGbglViYpLgPp1hyAFVbBIjWZ9aPL7kNe5b4fdsb4PfAai6iKmwftUdbTWB', 
                    0, (getdate()), 0, 0)");
        }

        public override void Down()
        {
            Execute("DELETE FROM [User];");
        }
    }
}
