using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Users;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Extensions.Export.Configuration;
using Signals.Core.Extensions.Export.Export.Excel;
using Signals.Core.Processes.Export;
using Signals.Core.Processing.Results;
using System.Collections.Generic;

namespace App.Domain.Processes.Users
{
    public partial class UserProcesses
    {
        public class ExportUsers : ExcelFileExportProcess<User>
        {
            [Import] private IUserRepository UserRepository { get; set; }

            /// <summary>
            /// Export configuration
            /// </summary>
            protected override IExportConfiguration<User> ExportConfiguration => new ExportConfiguration<User>
            {
                FileName = "users.csv",
                DataMapper = new Dictionary<string, System.Func<User, object>>
            {
                { "Name", user => user.Name },
                { "Email", user => user.Email },
            }
            };

            /// <summary>
            /// Auth process
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override FileResult Auth()
            {
                return Ok();
            }

            /// <summary>
            /// Validate process
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override FileResult Validate()
            {
                return Ok();
            }

            /// <summary>
            /// Resolve source process
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            protected override List<User> DataSource()
            {
                return UserRepository.GetAll();
            }
        }
    }
}
