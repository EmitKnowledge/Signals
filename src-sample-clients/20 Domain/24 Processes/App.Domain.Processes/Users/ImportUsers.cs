using App.Domain.Entities.Users;
using Signals.Core.Extensions.Import.Import.Excel;
using Signals.Core.Processes.Import;
using Signals.Core.Processing.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace App.Domain.Processes.Users
{
    public partial class UserProcesses
    {
        public class ImportUsers : ExcelFileImportProcess<Stream, User>
        {
            /// <summary>
            /// Import configuration
            /// </summary>
            protected override IImportConfiguration<User> ImportConfiguration => new Signals.Core.Extensions.Import.Configuration.ExcelImportConfiguration<User>
            {
                DataHandlers = new Dictionary<string[], Action<string[], User>>
                {
                    { new []{ "Name" }, (data, user) => user.Name = data.FirstOrDefault() },
                    { new []{ "Email" }, (data, user) => user.Email = data.FirstOrDefault() }
                },
                SheetIndex = 0,
                StartingRowIndex = 1
            };

            /// <summary>
            /// Auth process
            /// </summary>
            /// <param name="stream"></param>
            /// <returns></returns>
            public override ListResult<User> Auth(Stream stream)
            {
                return Ok();
            }

            /// <summary>
            /// Validate process
            /// </summary>
            /// <param name="stream"></param>
            /// <returns></returns>
            public override ListResult<User> Validate(Stream stream)
            {
                return Ok();
            }
        }
    }
}
