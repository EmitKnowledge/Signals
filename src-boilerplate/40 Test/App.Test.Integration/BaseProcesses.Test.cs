using System;
using System.Data.SqlClient;
using System.Reflection;
using App.Service.Configuration;
using App.Service.DomainEntities.Users;
using Dapper;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI;
using Signals.Aspects.DI.Autofac;
using Signals.Core.Configuration;
using Signals.Core.Configuration.Bootstrapping;

namespace App.Test.Integration
{
    public class TestBootstrapConfiguraiton : ApplicationBootstrapConfiguration
    {
        public void Bootstrap(params Assembly[] scanAssemblies)
        {
            Resolve(scanAssemblies);
        }

        protected override IServiceContainer Resolve(params Assembly[] scanAssemblies)
        {
            return base.Resolve(scanAssemblies);
        }
    }

    public class BaseProcessesTest
    {
        /// <summary>
        /// Executed when test class is initialized
        /// </summary>
        public BaseProcessesTest()
        {
            BusinessConfiguration.UseProvider(new FileConfigurationProvider
            {
                File = @"configs\business.config.json",
                Path = Environment.CurrentDirectory,
                ReloadOnAccess = false
            });

            ApplicationConfiguration.UseProvider(new FileConfigurationProvider
            {
                File = @"configs\application.config.json",
                Path = Environment.CurrentDirectory,
                ReloadOnAccess = false
            });

            TestBootstrapConfiguraiton config = new TestBootstrapConfiguraiton
            {
                RegistrationService = new RegistrationService(),
            };

            config.Bootstrap();

            SystemBootstrapper.Bootstrap(this);

            //CleanupDatabase();
            //RegisterDefaultUser();
        }

        /// <summary>
        /// Return the logged in user in the system
        /// </summary>
        public static User CurrentUser => new User
	    {
		    Id = 1,
		    Name = "Marjan Nikolovski",
		    Email = @"m4rjann",
		    Username = @"marjan@emitknowledge.com",
		    Password = @"123456"
	    };

	    private static void CleanupDatabase()
        {
            var scsb = new SqlConnectionStringBuilder(BusinessConfiguration.Instance.DatabaseConfiguration.ActiveConfiguration.ConnectionString);
            scsb.MultipleActiveResultSets = true;
            var cs = scsb.ConnectionString;
            using (var connection = new SqlConnection(cs))
            {
                connection.Open();

                // truncata token tables
                connection.Execute("truncate table OnNewUserRegisterEvent; DBCC CHECKIDENT ('[OnNewUserRegisterEvent]', RESEED, 0);");
                connection.Execute("truncate table OnPasswordResetEvent; DBCC CHECKIDENT ('[OnPasswordResetEvent]', RESEED, 0);");

                // truncata log and audit tables
                connection.Execute("truncate table ActionTrack; DBCC CHECKIDENT ('[ActionTrack]', RESEED, 0);");
                connection.Execute("truncate table SentMailTrack; DBCC CHECKIDENT ('[SentMailTrack]', RESEED, 0);");
                connection.Execute("truncate table SyncLog; DBCC CHECKIDENT ('[SyncLog]', RESEED, 0);");

                // truncata user table
                connection.Execute("truncate table ExternalConnection; DBCC CHECKIDENT ('[ExternalConnection]', RESEED, 0);");
                connection.Execute("truncate table UserSettings; DBCC CHECKIDENT ('[UserSettings]', RESEED, 0);");
                connection.Execute("truncate table UserImage; DBCC CHECKIDENT ('[UserImage]', RESEED, 0);");
                connection.Execute("delete from [User]; DBCC CHECKIDENT ('[User]', RESEED, 0);");

                connection.Close();
            }                
        }

        private static void RegisterDefaultUser()
        {
            var user = new User();
            user.Name = "Marjan Nikolovski";
            user.Email = @"marjan@emitknowledge.com";
            user.Username = @"m4rjann";
            user.Password = @"123456";
            //DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false, MarkUserAsVerified = true });            
        }
    }
}
