using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes;
using Xunit;

namespace App.Test.Integration
{
    public class UserProcessesTest : BaseProcessesTest
    {
        [Import] private ManualMediator _mediator { get; set; }

        public UserProcessesTest()
        {
        }

        [Fact]
        public void Do()
        {
        }

        //        [Fact]
        //        public void Check_If_All_Users_Are_Returned()
        //        {
        //            var newUsersUpperBound = 30;
        //            var users = new List<User>();
        //            for (int i = 0; i < newUsersUpperBound; i++)
        //            {
        //                var user = new User();
        //                user.Name = "Contact Emit Knowledge";
        //                user.Email = @"u" + i + "@emitknowledge.com";
        //                user.Username = @"u" + i;
        //                user.Password = @"123456";

        //                var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //                Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //                user.Id = registrationResult.Result;
        //                users.Add(user);
        //            }

        //            var getAllUsersResult = DomainServices.User.GetAllUsers(CurrentUser, queryOptions: null);
        //            Assert.Equal(getAllUsersResult.IsFaulted, false, getAllUsersResult.GetErrorsFaultMessage());
        //            Assert.Equal(getAllUsersResult.Result.Count, newUsersUpperBound + 1, @"Expected " + (newUsersUpperBound + 1) + " users and got none");

        //            var existingUsers = getAllUsersResult.Result;
        //            foreach (var user in users)
        //            {
        //                var existingUser = existingUsers.FirstOrDefault(x => x.Id == user.Id);
        //                Assert.NotNull(existingUser, @"Existng user can not be found");
        //                Assert.NotEqual(existingUser, null, @"Exisitng user can not be null");
        //                Assert.Equal(user.Name, existingUser.Name, @"New user and retrieved user should have the same name");
        //                Assert.Equal(user.Email, existingUser.Email, @"New user and retrieved user should have the same email");
        //                Assert.Equal(user.Username, existingUser.Username, @"New user and retrieved user should have the same username");
        //            }
        //        }

        //        [Fact]
        //        public void Check_If_All_Non_Confirmed_Users_Are_Returned()
        //        {
        //            var newUsersUpperBound = 30;
        //            var newConfirmedUsers = 10;
        //            var newNonConfirmedUsers = newUsersUpperBound - newConfirmedUsers;
        //            var users = new List<User>();
        //            for (int i = 0; i < newUsersUpperBound; i++)
        //            {
        //                var user = new User();
        //                user.Name = "Contact Emit Knowledge";
        //                user.Email = @"u" + i + "@emitknowledge.com";
        //                user.Username = @"u" + i;
        //                user.Password = @"123456";
        //                user.IsVerified = i < 10;
        //                var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //                Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //                user.Id = registrationResult.Result;
        //                if (!user.IsVerified)
        //                {
        //                    users.Add(user);
        //                }
        //            }

        //            var getAllUsersResult = DomainServices.User.GetAllNonConfirmedUsers(CurrentUser, queryOptions: null);
        //            Assert.Equal(getAllUsersResult.IsFaulted, false, getAllUsersResult.GetErrorsFaultMessage());
        //            Assert.Equal(getAllUsersResult.Result.Count, newNonConfirmedUsers, @"Expected " + newNonConfirmedUsers + " users and got none");

        //            var existingUsers = getAllUsersResult.Result;
        //            foreach (var user in users)
        //            {
        //                var existingUser = existingUsers.FirstOrDefault(x => x.Id == user.Id);
        //                Assert.NotNull(existingUser, @"Existng user can not be found");
        //                Assert.NotEqual(existingUser, null, @"Exisitng user can not be null");
        //                Assert.Equal(user.Name, existingUser.Name, @"New user and retrieved user should have the same name");
        //                Assert.Equal(user.Email, existingUser.Email, @"New user and retrieved user should have the same email");
        //                Assert.Equal(user.Username, existingUser.Username, @"New user and retrieved user should have the same username");
        //            }
        //        }

        //        [Fact]
        //        public void Register_Non_Existing_User()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });

        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            Assert.NotEqual(registrationResult.Result, 0, @"New user id can not be 0");

        //            var getUserResult = DomainServices.User.GetById(user, registrationResult.Result);
        //            var existingUser = getUserResult.Result;

        //            Assert.Equal(getUserResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            Assert.NotEqual(existingUser, null, @"Exisitng user can not be null");
        //            Assert.Equal(user.Name, existingUser.Name, @"New user and retrieved user should have the same name");
        //            Assert.Equal(user.Email, existingUser.Email, @"New user and retrieved user should have the same email");
        //            Assert.Equal(user.Username, existingUser.Username, @"New user and retrieved user should have the same username");
        //        }

        //        [Fact]
        //        public void Register_Existing_User()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, true, registrationResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void Check_If_User_Can_Register_With_An_Existing_Email_Account()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            var canRegisterResult = DomainServices.User.CanRegisterUser(user);
        //            Assert.Equal(canRegisterResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            Assert.Equal(canRegisterResult.Result, false, @"User's email is an existing one. Should not return true");
        //        }

        //        [Fact]
        //        public void Update_User_Username()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());

        //            user.Username = @"u2";
        //            var updateUserProfileResult = DomainServices.User.UpdateUserProfile(user, user);
        //            Assert.Equal(updateUserProfileResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());

        //            var existingUserResult = DomainServices.User.GetById(user, user.Id);
        //            Assert.Equal(existingUserResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            Assert.Equal(user.Username, existingUserResult.Result.Username, @"Username shoyld be same");
        //        }

        //        [Fact]
        //        public void Update_User_Name()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());

        //            user.Name = "Contact Emit Knowledge 2";
        //            var updateUserProfileResult = DomainServices.User.UpdateUserProfile(user, user);
        //            Assert.Equal(updateUserProfileResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());

        //            var existingUserResult = DomainServices.User.GetById(user, user.Id);
        //            Assert.Equal(existingUserResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            Assert.Equal(user.Username, existingUserResult.Result.Username, @"Username shoyld be same");
        //        }

        //        [Fact]
        //        public void Unsubsribe_From_Product_Emails()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());

        //            user.Settings = new DefaultUserSettings();
        //            user.Settings.SubscribeToProductEmails = false;

        //            var updateUserSettingsResult = DomainServices.User.UpdateSettings(user, user);
        //            Assert.Equal(updateUserSettingsResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());

        //            var getUserSettingsResult = DomainServices.User.GetUserSettings(user);
        //            Assert.Equal(getUserSettingsResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            Assert.Equal(getUserSettingsResult.Result.SubscribeToProductEmails, user.Settings.SubscribeToProductEmails, registrationResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void Add_Mailjet_External_Connection_For_User()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var mailjetExternalConnection = new ExternalConnection();
        //            mailjetExternalConnection.Provider = AppConfiguration.Instance.ExternalApisConfiguration.MailJet.Name;
        //            mailjetExternalConnection.UserId = user.Id;
        //            mailjetExternalConnection.ExternalUserId = (new Guid()).ToString();
        //            var addExternalContactResult = DomainServices.User.AddExternalConnection(user, mailjetExternalConnection);
        //            Assert.Equal(addExternalContactResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());

        //            var getExternalConnectionsResult = DomainServices.User.GetUserExternalConnections(user);
        //            Assert.Equal(getExternalConnectionsResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());

        //            var existingMailjetExternalConnection = getExternalConnectionsResult.Result.FirstOrDefault(x => x.Provider == AppConfiguration.Instance.ExternalApisConfiguration.MailJet.Name);
        //            Assert.NotNull(existingMailjetExternalConnection, @"Mailjet external connection was not found");
        //            Assert.Equal(mailjetExternalConnection.Provider, existingMailjetExternalConnection.Provider, @"Provider value not equal to Mailjet");
        //            Assert.Equal(mailjetExternalConnection.UserId, existingMailjetExternalConnection.UserId, @"User id value not equal to the provided user id");
        //            Assert.Equal(mailjetExternalConnection.ExternalUserId, existingMailjetExternalConnection.ExternalUserId, @"External user id value not equal to the provided one");
        //        }

        //        [Fact]
        //        public void Remove_Mailjet_External_Connection_For_User()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var mailjetExternalConnection = new ExternalConnection();
        //            mailjetExternalConnection.Provider = AppConfiguration.Instance.ExternalApisConfiguration.MailJet.Name;
        //            mailjetExternalConnection.UserId = user.Id;
        //            mailjetExternalConnection.ExternalUserId = (new Guid()).ToString();
        //            var addExternalContactResult = DomainServices.User.AddExternalConnection(user, mailjetExternalConnection);
        //            Assert.Equal(addExternalContactResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());

        //            var removeExternalContactResult = DomainServices.User.RemoveExternalConnection(user, mailjetExternalConnection);
        //            Assert.Equal(removeExternalContactResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());

        //            var getExternalConnectionsResult = DomainServices.User.GetUserExternalConnections(user);
        //            Assert.Equal(getExternalConnectionsResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());

        //            var existingMailjetExternalConnection = getExternalConnectionsResult.Result.FirstOrDefault(x => x.Provider == AppConfiguration.Instance.ExternalApisConfiguration.MailJet.Name);
        //            Assert.Null(existingMailjetExternalConnection, @"Mailjet external connection was found. It should have been removed");
        //        }

        //        [Fact]
        //        public void Get_User_Name_And_Email_By_Id()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var existingUserResult = DomainServices.User.GetById(user, user.Id);
        //            var existingUser = existingUserResult.Result;
        //            Assert.Equal(existingUserResult.IsFaulted, false, existingUserResult.GetErrorsFaultMessage());
        //            Assert.NotNull(existingUser, @"Existing user was not found with id: " + user.Id);
        //            Assert.Equal(user.Name, existingUser.Name);
        //            Assert.Equal(user.Email, existingUser.Email);
        //            Assert.Null(existingUser.Password);
        //            Assert.Null(existingUser.PasswordSalt);
        //        }

        //        [Fact]
        //        public void Get_User_By_Providing_A_Valid_Email()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var existingUserResult = DomainServices.User.GetUserByEmail(user.Email);
        //            var existingUser = existingUserResult.Result;
        //            Assert.Equal(existingUserResult.IsFaulted, false, existingUserResult.GetErrorsFaultMessage());
        //            Assert.NotNull(existingUser, @"Existing user was not found with email: " + user.Email);
        //            Assert.Equal(user.Name, existingUser.Name);
        //            Assert.Equal(user.Email, existingUser.Email);
        //            Assert.Null(existingUser.Password);
        //            Assert.Null(existingUser.PasswordSalt);
        //        }

        //        [Fact]
        //        public void Get_User_By_Providing_A_Invalid_Email()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var existingUserResult = DomainServices.User.GetUserByEmail(@"u@sdfsdfsd.com");
        //            var existingUser = existingUserResult.Result;
        //            Assert.Equal(existingUserResult.IsFaulted, false, existingUserResult.GetErrorsFaultMessage());
        //            Assert.Null(existingUser, @"Existing user was found with invalid email: u@sdfsdfsd.com");
        //        }

        //        [Fact]
        //        public void Get_User_By_Providing_A_Valid_Username()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var existingUserResult = DomainServices.User.GetUserByUsername(user.Username);
        //            var existingUser = existingUserResult.Result;
        //            Assert.Equal(existingUserResult.IsFaulted, false, existingUserResult.GetErrorsFaultMessage());
        //            Assert.NotNull(existingUser, @"Existing user was not found with username: " + user.Username);
        //            Assert.Equal(user.Name, existingUser.Name);
        //            Assert.Equal(user.Email, existingUser.Email);
        //            Assert.Null(existingUser.Password);
        //            Assert.Null(existingUser.PasswordSalt);
        //        }

        //        [Fact]
        //        public void Get_User_By_Providing_A_Invalid_Username()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var existingUserResult = DomainServices.User.GetUserByUsername(@"usdfsdfsd123");
        //            var existingUser = existingUserResult.Result;
        //            Assert.Equal(existingUserResult.IsFaulted, false, existingUserResult.GetErrorsFaultMessage());
        //            Assert.Null(existingUser, @"Existing user was found with invalid username: usdfsdfsd123");
        //        }

        //        [Fact]
        //        public void Search_Existing_Users_By_Username_Email_And_Name()
        //        {
        //            var users = new List<User>();
        //            for (int i = 0; i < 30; i++)
        //            {
        //                var user = new User();
        //                user.Name = "Contact Emit Knowledge";
        //                user.Email = @"ura" + i + "@emitknowledge.com";
        //                user.Username = @"ura" + i;
        //                user.Password = @"123456";

        //                if (i % 2 == 0)
        //                {
        //                    user.Email = @"email-ura-mode" + i + "@emitknowledge.com";
        //                    user.Username = @"username-ura-mode" + i;
        //                    user.Name = @"name-ura-mode" + i;
        //                }

        //                var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //                Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //                user.Id = registrationResult.Result;
        //                users.Add(user);
        //            }

        //            var searchUsersWithValidEmailResult = DomainServices.User.Search(CurrentUser, @"email-ura");
        //            Assert.Equal(searchUsersWithValidEmailResult.Result.Count, 15);

        //            var searchUsersWithValidUsernameResult = DomainServices.User.Search(CurrentUser, @"username-ura");
        //            Assert.Equal(searchUsersWithValidUsernameResult.Result.Count, 15);

        //            var searchUsersWithValidNameResult = DomainServices.User.Search(CurrentUser, @"name-ura");
        //            Assert.Equal(searchUsersWithValidNameResult.Result.Count, 15);

        //            var searchUsersResult = DomainServices.User.Search(CurrentUser, @"ura-mode");
        //            Assert.Equal(searchUsersResult.Result.Count, 15);

        //            searchUsersResult = DomainServices.User.Search(CurrentUser, @"ura");
        //            Assert.Equal(searchUsersResult.Result.Count, 30);
        //        }

        //        [Fact]
        //        public void User_Request_Confirmation_Email()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var requestConfirmationEmailResult = DomainServices.User.RequestConfirmationEmail(user, user);
        //            Assert.Equal(requestConfirmationEmailResult.IsFaulted, false, requestConfirmationEmailResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void User_Request_Password_Reset_Email()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var requestPasswordResetEmailResult = DomainServices.User.RequestResetPassword(user);
        //            Assert.Equal(requestPasswordResetEmailResult.IsFaulted, false, requestPasswordResetEmailResult.GetErrorsFaultMessage());
        //            Assert.NotNull(requestPasswordResetEmailResult.Result);
        //        }

        //        [Fact]
        //        public void Reset_User_Password_With_Existing_Valid_Password()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var changePasswordResults = DomainServices.User.ChangePassword(user, user, "123456", "654321");
        //            Assert.Equal(changePasswordResults.IsFaulted, false, changePasswordResults.GetErrorsFaultMessage());

        //            User existingUser = null;
        //            var verifyUserResult = DomainServices.User.VerifyUserCredentials(user.Email, @"654321", out existingUser);
        //            Assert.Equal(verifyUserResult.IsFaulted, false, verifyUserResult.GetErrorsFaultMessage());
        //            Assert.NotNull(existingUser, @"Password is not valid. User can not be verified");
        //            Assert.Equal(verifyUserResult.Result, true, @"Password is not valid. User can not be verified");
        //        }

        //        [Fact]
        //        public void Reset_User_Password_With_Existing_Invalid_Password()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var changePasswordResults = DomainServices.User.ChangePassword(user, user, "1234567", "654321");
        //            Assert.Equal(changePasswordResults.IsFaulted, true, changePasswordResults.GetErrorsFaultMessage());

        //            User existingUser = null;
        //            var verifyUserResult = DomainServices.User.VerifyUserCredentials(user.Email, @"654321", out existingUser);
        //            Assert.Null(existingUser);
        //            Assert.Equal(verifyUserResult.Result, false);
        //        }

        //        [Fact]
        //        public void Reset_User_Password_With_Valid_Token()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var requestPasswordResetEmailResult = DomainServices.User.RequestResetPassword(user);
        //            Assert.Equal(requestPasswordResetEmailResult.IsFaulted, false, requestPasswordResetEmailResult.GetErrorsFaultMessage());
        //            Assert.NotNull(requestPasswordResetEmailResult.Result);

        //            var token = requestPasswordResetEmailResult.Result;
        //            var resetPasswordWithTokenResult = DomainServices.User.ResetPassword(token, @"654321");
        //            Assert.Equal(resetPasswordWithTokenResult.IsFaulted, false, resetPasswordWithTokenResult.GetErrorsFaultMessage());

        //            User existingUser = null;
        //            var verifyUserResult = DomainServices.User.VerifyUserCredentials(user.Email, @"654321", out existingUser);
        //            Assert.Equal(verifyUserResult.IsFaulted, false, verifyUserResult.GetErrorsFaultMessage());
        //            Assert.NotNull(existingUser, @"Password is not valid. User can not be verified");
        //            Assert.Equal(verifyUserResult.Result, true, @"Password is not valid. User can not be verified");
        //        }

        //        [Fact]
        //        public void Reset_User_Password_With_Invalid_Token()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var token = @"aaaaaa";
        //            var resetPasswordWithTokenResult = DomainServices.User.ResetPassword(token, @"654321");
        //            Assert.Equal(resetPasswordWithTokenResult.IsFaulted, true, resetPasswordWithTokenResult.GetErrorsFaultMessage());

        //            User existingUser = null;
        //            var verifyUserResult = DomainServices.User.VerifyUserCredentials(user.Email, @"654321", out existingUser);
        //            Assert.Null(existingUser);
        //            Assert.Equal(verifyUserResult.Result, false);
        //        }

        //        [Fact]
        //        public void Revoke_Password_Request_Change_Token()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var requestPasswordResetEmailResult = DomainServices.User.RequestResetPassword(user);
        //            Assert.Equal(requestPasswordResetEmailResult.IsFaulted, false, requestPasswordResetEmailResult.GetErrorsFaultMessage());
        //            Assert.NotNull(requestPasswordResetEmailResult.Result);

        //            var token = requestPasswordResetEmailResult.Result;
        //            var revokePasswordChangeResult = DomainServices.User.RevokePasswordChange(user, token);
        //            Assert.Equal(revokePasswordChangeResult.IsFaulted, false, revokePasswordChangeResult.GetErrorsFaultMessage());
        //            Assert.Equal(revokePasswordChangeResult.Result, true, revokePasswordChangeResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void Get_Password_Reset_Token_From_Valid_Token()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var requestPasswordResetEmailResult = DomainServices.User.RequestResetPassword(user);
        //            Assert.Equal(requestPasswordResetEmailResult.IsFaulted, false, requestPasswordResetEmailResult.GetErrorsFaultMessage());
        //            Assert.NotNull(requestPasswordResetEmailResult.Result);

        //            var token = requestPasswordResetEmailResult.Result;
        //            var getPasswordResetTokenResult = DomainServices.User.GetPasswordResetToken(user, token);
        //            Assert.Equal(getPasswordResetTokenResult.IsFaulted, false, getPasswordResetTokenResult.GetErrorsFaultMessage());
        //            Assert.NotNull(getPasswordResetTokenResult.Result);
        //            Assert.Equal(getPasswordResetTokenResult.Result.Token, token);
        //        }

        //        [Fact]
        //        public void Get_Password_Reset_Token_From_Invalid_Token()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var token = @"123456";
        //            var getPasswordResetTokenResult = DomainServices.User.GetPasswordResetToken(user, token);
        //            Assert.Equal(getPasswordResetTokenResult.IsFaulted, false, getPasswordResetTokenResult.GetErrorsFaultMessage());
        //            Assert.Null(getPasswordResetTokenResult.Result);
        //        }

        //        [Fact]
        //        public void Get_User_Registration_Token_For_Newly_Registered_User()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = true });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var registrationTokenResult = DomainServices.User.GetUserRegistrationToken(user);
        //            Assert.Equal(registrationTokenResult.IsFaulted, false, registrationTokenResult.GetErrorsFaultMessage());
        //            Assert.NotNull(registrationTokenResult.Result);
        //            Assert.Equal(registrationTokenResult.Result.UserId, user.Id);
        //        }

        //        [Fact]
        //        public void Is_Existing_Valid_Registration_Token()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = true });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var registrationTokenResult = DomainServices.User.GetUserRegistrationToken(user);
        //            Assert.Equal(registrationTokenResult.IsFaulted, false, registrationTokenResult.GetErrorsFaultMessage());
        //            Assert.NotNull(registrationTokenResult.Result);
        //            Assert.Equal(registrationTokenResult.Result.UserId, user.Id);

        //            var isExistingRegistrationTokenResult = DomainServices.User.IsExistingRegistrationToken(registrationTokenResult.Result.Token);
        //            Assert.Equal(isExistingRegistrationTokenResult.IsFaulted, false, isExistingRegistrationTokenResult.GetErrorsFaultMessage());
        //            Assert.Equal(isExistingRegistrationTokenResult.Result, true, isExistingRegistrationTokenResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void Is_Existing_Invalid_Registration_Token()
        //        {
        //            var token = @"123456";
        //            var isExistingRegistrationTokenResult = DomainServices.User.IsExistingRegistrationToken(token);
        //            Assert.Equal(isExistingRegistrationTokenResult.IsFaulted, false, isExistingRegistrationTokenResult.GetErrorsFaultMessage());
        //            Assert.Equal(isExistingRegistrationTokenResult.Result, false, isExistingRegistrationTokenResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void Invalidate_Registration_Token()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = true });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var registrationTokenResult = DomainServices.User.GetUserRegistrationToken(user);
        //            Assert.Equal(registrationTokenResult.IsFaulted, false, registrationTokenResult.GetErrorsFaultMessage());
        //            Assert.NotNull(registrationTokenResult.Result);
        //            Assert.Equal(registrationTokenResult.Result.UserId, user.Id);

        //            var invalidateRegistrationTokenResult = DomainServices.User.InvalidateRegistrationToken(registrationTokenResult.Result.Token);
        //            Assert.Equal(invalidateRegistrationTokenResult.IsFaulted, false, invalidateRegistrationTokenResult.GetErrorsFaultMessage());
        //            Assert.Equal(invalidateRegistrationTokenResult.Result, true, invalidateRegistrationTokenResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void Get_User_From_Registration_Token()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = true });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var registrationTokenResult = DomainServices.User.GetUserRegistrationToken(user);
        //            Assert.Equal(registrationTokenResult.IsFaulted, false, registrationTokenResult.GetErrorsFaultMessage());
        //            Assert.NotNull(registrationTokenResult.Result);
        //            Assert.Equal(registrationTokenResult.Result.UserId, user.Id);

        //            var getUserFromRegistrationTokenResult = DomainServices.User.GetUserFromRegistrationToken(registrationTokenResult.Result.Token);
        //            Assert.Equal(getUserFromRegistrationTokenResult.IsFaulted, false, getUserFromRegistrationTokenResult.GetErrorsFaultMessage());
        //            Assert.NotNull(getUserFromRegistrationTokenResult.Result);
        //            Assert.Equal(getUserFromRegistrationTokenResult.Result.Id, user.Id, getUserFromRegistrationTokenResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void Is_Existing_User_Check_With_Existing_User()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = true });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var isExistingUserResult = DomainServices.User.IsExistingUser(user, user);
        //            Assert.Equal(isExistingUserResult.IsFaulted, false, isExistingUserResult.GetErrorsFaultMessage());
        //            Assert.Equal(isExistingUserResult.Result, true, isExistingUserResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void Is_Existing_User_Check_With_Nonexisting_User()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var isExistingUserResult = DomainServices.User.IsExistingUser(user, user);
        //            Assert.Equal(isExistingUserResult.IsFaulted, false, isExistingUserResult.GetErrorsFaultMessage());
        //            Assert.Equal(isExistingUserResult.Result, false, isExistingUserResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void Verify_User_With_Correct_Credentials()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            User existingUser = null;
        //            var verifyUserResult = DomainServices.User.VerifyUserCredentials(user.Email, @"123456", out existingUser);
        //            Assert.Equal(verifyUserResult.IsFaulted, false, verifyUserResult.GetErrorsFaultMessage());
        //            Assert.NotNull(existingUser, @"Password is not valid. User can not be verified");
        //            Assert.Equal(verifyUserResult.Result, true, @"Password is not valid. User can not be verified");
        //        }

        //        [Fact]
        //        public void Verify_User_With_Incorrect_Credentials()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            User existingUser = null;
        //            var verifyUserResult = DomainServices.User.VerifyUserCredentials(user.Email, @"asdasdjyu", out existingUser);
        //            Assert.Equal(verifyUserResult.IsFaulted, false, verifyUserResult.GetErrorsFaultMessage());
        //            Assert.Null(existingUser, @"Password is not valid. User can not be verified");
        //            Assert.Equal(verifyUserResult.Result, false, @"Password is not valid. User can not be verified");
        //        }

        //        [Fact]
        //        public void Check_If_Verified_User_Is_Verified()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";
        //            user.IsVerified = true;

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var isVerifiedUserResult = DomainServices.User.IsUserVerified(user, user);
        //            Assert.Equal(isVerifiedUserResult.IsFaulted, false, isVerifiedUserResult.GetErrorsFaultMessage());
        //            Assert.Equal(isVerifiedUserResult.Result, true, isVerifiedUserResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void Check_If_Nonverified_User_Is_Verified()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var isVerifiedUserResult = DomainServices.User.IsUserVerified(user, user);
        //            Assert.Equal(isVerifiedUserResult.IsFaulted, false, isVerifiedUserResult.GetErrorsFaultMessage());
        //            Assert.Equal(isVerifiedUserResult.Result, false, isVerifiedUserResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void Mark_User_As_Verified()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = false });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var markUserAsVerifiedResult = DomainServices.User.MarkUserAsVerfied(user);
        //            Assert.Equal(markUserAsVerifiedResult.IsFaulted, false, markUserAsVerifiedResult.GetErrorsFaultMessage());

        //            var isVerifiedUserResult = DomainServices.User.IsUserVerified(user, user);
        //            Assert.Equal(isVerifiedUserResult.IsFaulted, false, isVerifiedUserResult.GetErrorsFaultMessage());
        //            Assert.Equal(isVerifiedUserResult.Result, true, isVerifiedUserResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void Mark_User_As_Verified_From_Token()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = true });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var registrationTokenResult = DomainServices.User.GetUserRegistrationToken(user);
        //            Assert.Equal(registrationTokenResult.IsFaulted, false, registrationTokenResult.GetErrorsFaultMessage());
        //            Assert.NotNull(registrationTokenResult.Result);
        //            Assert.Equal(registrationTokenResult.Result.UserId, user.Id);

        //            var markUserAsVerifiedResult = DomainServices.User.MarkUserAsVerfied(registrationTokenResult.Result.Token);
        //            Assert.Equal(markUserAsVerifiedResult.IsFaulted, false, markUserAsVerifiedResult.GetErrorsFaultMessage());

        //            var isVerifiedUserResult = DomainServices.User.IsUserVerified(user, user);
        //            Assert.Equal(isVerifiedUserResult.IsFaulted, false, isVerifiedUserResult.GetErrorsFaultMessage());
        //            Assert.Equal(isVerifiedUserResult.Result, true, isVerifiedUserResult.GetErrorsFaultMessage());
        //        }

        //        [Fact]
        //        public void Create_Default_Profile_Picture_For_New_User()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = true });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var createDefaultProfilePictureResult = DomainServices.User.CreateDefaultProfileImage(user);
        //            Assert.Equal(createDefaultProfilePictureResult.IsFaulted, false, createDefaultProfilePictureResult.GetErrorsFaultMessage());
        //            Assert.NotNull(createDefaultProfilePictureResult.Result);
        //        }

        //        [Fact]
        //        public void Update_Default_Profile_Picture_For_User()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = true });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var createDefaultProfilePictureResult = DomainServices.User.CreateDefaultProfileImage(user);
        //            Assert.Equal(createDefaultProfilePictureResult.IsFaulted, false, createDefaultProfilePictureResult.GetErrorsFaultMessage());
        //            Assert.NotNull(createDefaultProfilePictureResult.Result);

        //            var userImage = new UserImage();
        //            userImage.UserId = user.Id;
        //            userImage.ImageOriginal = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        //            userImage.ImageSizeVarationA = new byte[] { 0, 1, 2, 3, 4, 5, 6 };
        //            userImage.ImageSizeVarationB = new byte[] { 0, 1, 2, 3, 4 };
        //            userImage.ImageSizeVarationC = new byte[] { 0, 1, 2 };

        //            var updateDefaultProfilePictureResult = DomainServices.User.UpdateProfileImage(user, userImage);
        //            Assert.Equal(updateDefaultProfilePictureResult.IsFaulted, false, updateDefaultProfilePictureResult.GetErrorsFaultMessage());

        //            // check original image
        //            var getUserProfilePictureResult = DomainServices.User.GetUserImage(user, UserImageDimensionsEnum.ImageOriginal);
        //            Assert.Equal(getUserProfilePictureResult.IsFaulted, false, getUserProfilePictureResult.GetErrorsFaultMessage());
        //            Assert.NotNull(getUserProfilePictureResult.Result);

        //            for (int i = 0; i < userImage.ImageOriginal.Length; i++)
        //            {
        //                Assert.Equal(userImage.ImageOriginal[i], getUserProfilePictureResult.Result.ImageOriginal[i]);
        //            }

        //            // check image variation A
        //            getUserProfilePictureResult = DomainServices.User.GetUserImage(user, UserImageDimensionsEnum.ImageSizeVarationA);
        //            Assert.Equal(getUserProfilePictureResult.IsFaulted, false, getUserProfilePictureResult.GetErrorsFaultMessage());
        //            Assert.NotNull(getUserProfilePictureResult.Result);

        //            for (int i = 0; i < userImage.ImageSizeVarationA.Length; i++)
        //            {
        //                Assert.Equal(userImage.ImageSizeVarationA[i], getUserProfilePictureResult.Result.ImageSizeVarationA[i]);
        //            }

        //            // check image variation B
        //            getUserProfilePictureResult = DomainServices.User.GetUserImage(user, UserImageDimensionsEnum.ImageSizeVarationB);
        //            Assert.Equal(getUserProfilePictureResult.IsFaulted, false, getUserProfilePictureResult.GetErrorsFaultMessage());
        //            Assert.NotNull(getUserProfilePictureResult.Result);

        //            for (int i = 0; i < userImage.ImageSizeVarationB.Length; i++)
        //            {
        //                Assert.Equal(userImage.ImageSizeVarationB[i], getUserProfilePictureResult.Result.ImageSizeVarationB[i]);
        //            }

        //            // check image variation C
        //            getUserProfilePictureResult = DomainServices.User.GetUserImage(user, UserImageDimensionsEnum.ImageSizeVarationC);
        //            Assert.Equal(getUserProfilePictureResult.IsFaulted, false, getUserProfilePictureResult.GetErrorsFaultMessage());
        //            Assert.NotNull(getUserProfilePictureResult.Result);

        //            for (int i = 0; i < userImage.ImageSizeVarationC.Length; i++)
        //            {
        //                Assert.Equal(userImage.ImageSizeVarationC[i], getUserProfilePictureResult.Result.ImageSizeVarationC[i]);
        //            }
        //        }

        //        [Fact]
        //        public void Get_User_Profile_Picture()
        //        {
        //            var user = new User();
        //            user.Name = "Contact Emit Knowledge";
        //            user.Email = @"u1@emitknowledge.com";
        //            user.Username = @"u1";
        //            user.Password = @"123456";

        //            var registrationResult = DomainServices.User.Register(user, new RegistrationOptions { ShouldSendRegistrationEmail = true });
        //            Assert.Equal(registrationResult.IsFaulted, false, registrationResult.GetErrorsFaultMessage());
        //            user.Id = registrationResult.Result;

        //            var createDefaultProfilePictureResult = DomainServices.User.CreateDefaultProfileImage(user);
        //            Assert.Equal(createDefaultProfilePictureResult.IsFaulted, false, createDefaultProfilePictureResult.GetErrorsFaultMessage());
        //            Assert.NotNull(createDefaultProfilePictureResult.Result);

        //            var userImage = new UserImage();
        //            userImage.UserId = user.Id;
        //            userImage.ImageOriginal = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        //            userImage.ImageSizeVarationA = new byte[] { 0, 1, 2, 3, 4, 5, 6 };
        //            userImage.ImageSizeVarationB = new byte[] { 0, 1, 2, 3, 4 };
        //            userImage.ImageSizeVarationC = new byte[] { 0, 1, 2 };

        //            var updateDefaultProfilePictureResult = DomainServices.User.UpdateProfileImage(user, userImage);
        //            Assert.Equal(updateDefaultProfilePictureResult.IsFaulted, false, updateDefaultProfilePictureResult.GetErrorsFaultMessage());

        //            // check original image
        //            var getUserProfilePictureResult = DomainServices.User.GetUserImage(user, UserImageDimensionsEnum.ImageOriginal);
        //            Assert.Equal(getUserProfilePictureResult.IsFaulted, false, getUserProfilePictureResult.GetErrorsFaultMessage());
        //            Assert.NotNull(getUserProfilePictureResult.Result);

        //            for (int i = 0; i < userImage.ImageOriginal.Length; i++)
        //            {
        //                Assert.Equal(userImage.ImageOriginal[i], getUserProfilePictureResult.Result.ImageOriginal[i]);
        //            }

        //            // check image variation A
        //            getUserProfilePictureResult = DomainServices.User.GetUserImage(user, UserImageDimensionsEnum.ImageSizeVarationA);
        //            Assert.Equal(getUserProfilePictureResult.IsFaulted, false, getUserProfilePictureResult.GetErrorsFaultMessage());
        //            Assert.NotNull(getUserProfilePictureResult.Result);

        //            for (int i = 0; i < userImage.ImageSizeVarationA.Length; i++)
        //            {
        //                Assert.Equal(userImage.ImageSizeVarationA[i], getUserProfilePictureResult.Result.ImageSizeVarationA[i]);
        //            }

        //            // check image variation B
        //            getUserProfilePictureResult = DomainServices.User.GetUserImage(user, UserImageDimensionsEnum.ImageSizeVarationB);
        //            Assert.Equal(getUserProfilePictureResult.IsFaulted, false, getUserProfilePictureResult.GetErrorsFaultMessage());
        //            Assert.NotNull(getUserProfilePictureResult.Result);

        //            for (int i = 0; i < userImage.ImageSizeVarationB.Length; i++)
        //            {
        //                Assert.Equal(userImage.ImageSizeVarationB[i], getUserProfilePictureResult.Result.ImageSizeVarationB[i]);
        //            }

        //            // check image variation C
        //            getUserProfilePictureResult = DomainServices.User.GetUserImage(user, UserImageDimensionsEnum.ImageSizeVarationC);
        //            Assert.Equal(getUserProfilePictureResult.IsFaulted, false, getUserProfilePictureResult.GetErrorsFaultMessage());
        //            Assert.NotNull(getUserProfilePictureResult.Result);

        //            for (int i = 0; i < userImage.ImageSizeVarationC.Length; i++)
        //            {
        //                Assert.Equal(userImage.ImageSizeVarationC[i], getUserProfilePictureResult.Result.ImageSizeVarationC[i]);
        //            }
        //        }
    }
}