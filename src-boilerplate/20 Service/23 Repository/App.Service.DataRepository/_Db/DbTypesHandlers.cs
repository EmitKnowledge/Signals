using Dapper;
using NodaTime;
using System;
using System.Data;

namespace App.Service.DataRepository._Db
{
    public class InstantTimeHandler : SqlMapper.TypeHandler<Instant>
    {
        public override void SetValue(IDbDataParameter parameter, Instant value)
        {
            parameter.Value = value.ToDateTimeUtc();
        }

        public override Instant Parse(object value)
        {
            if (value == null) return Instant.MinValue;
            var dateTime = Convert.ToDateTime(value);
            return Instant.FromDateTimeUtc(dateTime);
        }
    }
}