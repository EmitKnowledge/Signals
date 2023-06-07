using System.Collections.Generic;
using System.Linq;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Input.Http.Models;
using Signals.Core.Processing.Results;

namespace Signals.Clients.WebApi.BusinessProcesses.Files
{
    public class StoreFiles : BusinessProcess<ListResult<InputFile>>
    {
        [Import] private IHttpContextWrapper HttpContextWrapper { get; set; }

        public override ListResult<InputFile> Auth()
        {
            return Ok();
        }

        public override ListResult<InputFile> Validate()
        {
            return Ok();
        }

        public override ListResult<InputFile> Handle()
        {
            var files = HttpContextWrapper.Files?.ToList() ?? new List<InputFile>();

            return files;
        }
    }
}
