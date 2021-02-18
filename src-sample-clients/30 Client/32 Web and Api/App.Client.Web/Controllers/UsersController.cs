using App.Domain.Processes.Users;
using Microsoft.AspNetCore.Mvc;
using Signals.Aspects.DI;
using Signals.Core.Processes;

namespace App.Client.Web.Controllers
{
    public class UsersController : Controller
    {
        private Mediator Mediator => SystemBootstrapper.GetInstance<Mediator>();

        public ActionResult Import()
        {
            using (var file = Request.Form.Files[0].OpenReadStream())
            {
                var usersResult = Mediator.For<UserProcesses.ImportUsers>().With(file);
                if (usersResult.IsFaulted) return BadRequest();
                var users = usersResult.Result;

                return Json(users);
            }
        }

        public ActionResult Export()
        {
            var usersFileResult = Mediator.For<UserProcesses.ExportUsers>().With();
            if (usersFileResult.IsFaulted) return BadRequest();
            var usersFile = usersFileResult.Result;

            return File(usersFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
