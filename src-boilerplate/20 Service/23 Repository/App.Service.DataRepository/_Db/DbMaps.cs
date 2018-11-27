using App.Service.DomainEntities.Events.Users;
using App.Service.DomainEntities.Users;
using DapperExtensions.Mapper;

namespace App.Service.DataRepository._Db
{
    /// <summary>
    /// Mapping for user table
    /// </summary>
    public class UserMapper : ClassMapper<User>
    {
        public UserMapper()
        {
            Table("User");
            Map(m => m.Settings).Ignore();
            Map(m => m.ExternalConnections).Ignore();
            AutoMap();
        }
    }

    /// <summary>
    /// Mapping for OnNewUserRegisterEvent table
    /// </summary>
    public class OnNewUserRegisterEventMapper : ClassMapper<OnNewUserRegisterEvent>
    {
        public OnNewUserRegisterEventMapper()
        {
            Table("OnNewUserRegisterEvent");
            Map(m => m.EventName).Ignore();
            AutoMap();
        }
    }

    /// <summary>
    /// Mapping for OnPasswordResetEvent table
    /// </summary>
    public class OnPasswordResetEventMapper : ClassMapper<OnPasswordResetEvent>
    {
        public OnPasswordResetEventMapper()
        {
            Table("OnPasswordResetEvent");
            Map(m => m.EventName).Ignore();
            AutoMap();
        }
    }
}