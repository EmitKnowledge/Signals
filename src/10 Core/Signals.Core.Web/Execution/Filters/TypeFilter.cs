﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Signals.Core.Business.Api;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Web.Http;

namespace Signals.Core.Web.Execution.Filters
{
    /// <summary>
    /// Process type filter
    /// </summary>
    public class TypeFilter : IFilter
    {
        /// <summary>
        /// Check if the process type is correct based on the request
        /// </summary>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsCorrectProcessType(Type type, IHttpContextWrapper context)
        {
            return type.GetInterfaces().Contains(typeof(IApiProcess));
        }
    }
}
