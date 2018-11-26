using Microsoft.AspNetCore.Mvc;
using Signals.Aspects.Bootstrap;
using Signals.Clients.NetCoreWeb.BusinessProcesses;
using Signals.Core.Processes;
using Signals.Core.Processing.Results;
using System.IO;
using Microsoft.AspNetCore.Http;
using Signals.Aspects.Localization;
using FileResult = Signals.Core.Processing.Results.FileResult;

namespace Signals.Clients.NetCoreWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(ILocalizationProvider provider)
        {
            var mediator = SystemBootstrapper.GetInstance<ManualMediator>();
            var stream = mediator.Dispatch<UsersExportProcess, FileResult>();
            //return File(stream, System.Net.Mime.MediaTypeNames.Application.Octet, "users.pdf");
            return View();
        }

        [HttpPost]
        public IActionResult Import(IFormFile file)
        {
            var mediator = SystemBootstrapper.GetInstance<ManualMediator>();
            var stream = new MemoryStream();
            file.CopyTo(stream);
            var users = mediator.Dispatch<UsersImportProcess, MemoryStream, ListResult<BusinessProcesses.User>>(stream);
            return Ok(users);
        }
    }
}
