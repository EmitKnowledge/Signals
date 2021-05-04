using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.Tests.Core.Web.Web.Controllers
{
    public class PingController : Controller
    {
        public IActionResult Index()
        {
            return Json(new { Result = true });
        }
    }
}
