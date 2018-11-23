using Signals.Core.Business.Import;
using Signals.Core.Extensions.ImportExport.Configuration.Import;
using Signals.Core.Extensions.ImportExport.Import.Excel;
using Signals.Core.Processing.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Signals.Clients.NetCoreWeb.BusinessProcesses
{
    public class UsersImportProcess : ExcelFileImportProcess<Stream, User>
    {
        protected override IImportConfiguration<User> ImportConfiguration => new ExcelImportConfiguration<User>
        {
            DataHandlers = new Dictionary<string[], Action<string[], User>>
            {
                {
                    new [] {"First name"}, (values, user) => user.FirstName = values.FirstOrDefault()
                },
                {
                    new [] {"Last name"}, (values, user) => user.LastName = values.FirstOrDefault()
                }
            }
        };

        public override ListResult<User> Authenticate(Stream stream)
        {
            return new ListResult<User>();
        }

        public override ListResult<User> Authorize(Stream stream)
        {
            return new ListResult<User>();
        }

        public override ListResult<User> Validate(Stream stream)
        {
            return new ListResult<User>();
        }
    }
}
