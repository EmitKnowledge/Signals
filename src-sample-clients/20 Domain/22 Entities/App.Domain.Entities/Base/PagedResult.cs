using System;
using System.Collections.Generic;

namespace App.Domain.Entities.Base
{
    public class PagedResult<T>
    {
        public List<T> Result { get; set; }

        public int TotalCount { get; set; }

        public static implicit operator ValueTuple<List<T>, int>(PagedResult<T> result)
        {
            return (result.Result, result.TotalCount);
        }

        public static implicit operator PagedResult<T>(ValueTuple<List<T>, int> result)
        {
            return new PagedResult<T>
            {
                Result = result.Item1,
                TotalCount = result.Item2
            };
        }

        public object Select(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}