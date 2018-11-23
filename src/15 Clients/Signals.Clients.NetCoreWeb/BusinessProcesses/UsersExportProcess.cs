using Signals.Core.Business.Export;
using Signals.Core.Extensions.ImportExport.Configuration.Export;
using Signals.Core.Extensions.ImportExport.Export.Excel;
using Signals.Core.Processing.Results;
using System;
using System.Collections.Generic;

namespace Signals.Clients.NetCoreWeb.BusinessProcesses
{
    public class UsersExportProcess : ExcelFileExportProcess<User>
    {
        public override FileResult Authenticate()
        {
            return new FileResult();
        }

        public override FileResult Authorize()
        {
            return new FileResult();
        }

        public override FileResult Validate()
        {
            return new FileResult();
        }

        protected override List<User> DataSource()
        {
            return new List<User>
            {
                new User
                {
                    FirstName = "Petko",
                    LastName = "Smith"
                },
                new User
                {
                    FirstName = "Mirche",
                    LastName = "Kotzias"
                },
                new User
                {
                    FirstName = "Ratko",
                    LastName = "Orben"
                },
                new User
                {
                    FirstName = "Vasil",
                    LastName = "Tupurkovski"
                }
            };
        }

        protected override IExportConfiguration<User> ExportConfiguration => new PdfExportConfiguration<User>
        {
            FileName = "Users.xlsx",
            DataMapper = new Dictionary<string, Func<User, object>>
            {
                {
                    "First name", x => x.FirstName
                },
                {
                    "Last name", x => x.LastName
                },
                {
                    "Full name", x => x.FirstName + " " + x.LastName
                }
            }
        };
    }
}